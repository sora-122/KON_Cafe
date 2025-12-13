using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// カフェ運営のデータとビジネスロジックを保持する Model
/// スロット管理と在庫管理を行う
/// </summary>
public class CafeModel : IDisposable
{
    // 在庫更新イベント (ItemID, 新しい在庫数)
    public event Action<string, int> OnStockChanged;

    // タスク増減イベント
    public event Action<CustomerTask> OnTaskAdded;
    public event Action<string> OnTaskRemoved;

    // スコア更新イベント
    public event Action<int> OnScoreChanged;

    // ゲーム時間関連イベント
    public event Action<float> OnGameTimeUpdated;
    public event Action<GameResult> OnGameTimeOver;

    // スロット (本来は動的に増えるが、現時点では固定数2で実装)
    private readonly CookingSlot[] _slots;

    //  在庫データ (ItemId -> Count)
    private readonly Dictionary<string, int> _stockInventory = new Dictionary<string, int>();

    // アクティブなタスクリスト (TaskID -> CustomerTask)
    private readonly Dictionary<string, CustomerTask> _activeTasks = new Dictionary<string, CustomerTask>();

    // 現在のスコア
    private int _currentScore = 0;

    // ゲーム設定と状態
    private const float k_GameDuration = 60.0f; // 1プレイ60秒
    private float _gameRemainingTime;

    // 非同期処理キャンセル用トークン
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    // マスターデータ参照
    private readonly IMasterDataRepository _masterData;

    // ゲーム状態
    private bool _isPlaying = false;

    // カフェ運営シーン (CafeOperation.unity) 起動時 (初期化時) に一度だけ実行するメソッド
    public CafeModel(IMasterDataRepository masterData)
    {
        _masterData = masterData;

        // スロット初期化 (2つ)
        _slots = new CookingSlot[2];
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i] = new CookingSlot();
        }
    }


    // --- ゲームループ開始 ---

    /// <summary>
    /// カフェ運営 (来店ループ) を開始する
    /// </summary>
    public void StartGameLoop()
    {
        if (_isPlaying) return;
        _isPlaying = true;

        // 制限時間初期化
        _gameRemainingTime = k_GameDuration;
        OnGameTimeUpdated?.Invoke(_gameRemainingTime);

        // 来店ループを Fire and Forget で開始
        SpawnLoopAsync(_cts.Token).Forget();

        // 時間経過ループを Fire and Forget で開始
        TimeUpdateLoopAsync(_cts.Token).Forget();
    }

    /// <summary>
    /// 定期的にアニマルを来店させるループ
    /// </summary>
    private async UniTask SpawnLoopAsync(CancellationToken token)
    {
        while (_isPlaying && !token.IsCancellationRequested)
        {
            // 3～5秒の範囲でランダムな時間待機
            float waitTime = UnityEngine.Random.Range(3.0f, 5.0f);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);

            // 来店処理
            TrySpawnCustomer();
        }
    }

    /// <summary>
    /// タスクの制限時間を管理するループ
    /// </summary>
    private async UniTask TimeUpdateLoopAsync(CancellationToken token)
    {
        // 削除対象を一時保管するリスト (ループ内でのコレクション操作エラー防止)
        List<string> expiredTaskIds = new List<string>();

        while (_isPlaying && !token.IsCancellationRequested)
        {
            // 次フレームまで待機 (Updateタイミング)
            await UniTask.Yield(PlayerLoopTiming.Update, token);

            float deltaTime = Time.deltaTime;

            // ゲーム全体の時間経過と終了判定
            _gameRemainingTime -= deltaTime;
            OnGameTimeUpdated?.Invoke(_gameRemainingTime);

            if (_gameRemainingTime <= 0)
            {
                FinishGame();
                break; // ループを抜ける
            }

            expiredTaskIds.Clear();

            // 全タスクの時間を減らす
            foreach (var kvp in _activeTasks)
            {
                var task = kvp.Value;
                task.RemainingTime -= deltaTime;

                // 時間切れチェック
                if (task.RemainingTime <= 0)
                {
                    expiredTaskIds.Add(task.TaskId);
                }
            }

            // 時間切れタスクの削除処理
            foreach (var id in expiredTaskIds)
            {
                RemoveTask(id);
            }
        }
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    private void FinishGame()
    {
        _isPlaying = false;
        _gameRemainingTime = 0;
        OnGameTimeUpdated?.Invoke(0);

        // リザルト計算
        var result = CalculateResult();
        OnGameTimeOver?.Invoke(result);
    }

    /// <summary>
    /// リザルト計算処理
    /// </summary>
    private GameResult CalculateResult()
    {
        // ランク判定 (仮)
        // S: 1000点以上, A: 700点以上, B: 400点以上, C: それ未満
        string rank;
        float bonusRate;

        if (_currentScore >= 1000) { rank = "S"; bonusRate = 1.2f; }
        else if (_currentScore >= 700) { rank = "A"; bonusRate = 1.0f; }
        else if (_currentScore >= 400) { rank = "B"; bonusRate = 0.8f; }
        else { rank = "C"; bonusRate = 0.5f; }

        // 報酬計算 (仮: スコア * クリア時ランク補正)
        int money = Mathf.FloorToInt(_currentScore * bonusRate);
        int exp = Mathf.FloorToInt(_currentScore * bonusRate);

        return new GameResult
        {
            Score = _currentScore,
            Rank = rank,
            Money = money,
            Experience = exp
        };
    }

    /// <summary>
    /// タスクを削除する (時間切れ含む)
    /// </summary>
    private void RemoveTask(string taskId)
    {
        if (_activeTasks.ContainsKey(taskId))
        {
            _activeTasks.Remove(taskId);
            OnTaskRemoved?.Invoke(taskId);
            // 時間切れの場合はスコア加算無し
        }
    }

    /// <summary>
    /// 来店判定とタスク生成を行う
    /// </summary>
    private void TrySpawnCustomer()
    {
        // 最大来店数制限 (仮: 基本5体 + スコア/500)
        int maxCustomer = 5 + (_currentScore / 500);
        if (_activeTasks.Count >= maxCustomer) return;

        // 1. レアリティ抽選
        AnimalRarity rarity = GetRandomRarity();

        // 2. 抽選されたレアリティのアニマルをランダムに取得
        AnimalData animal = GetRandomAnimal(rarity);
        if (animal == null) return; // データ不足等の場合

        // 3. ランダムなメニューを注文 (将来はアニマルの好物等に対応予定)
        MenuItemData menu = GetRandomMenu();
        if (menu == null) return;

        // 4. タスク生成と通知
        // 制限時間 (25秒～35秒) を設定してタスク生成
        float timeLimit = UnityEngine.Random.Range(25.0f, 35.0f);
        var task = new CustomerTask(Guid.NewGuid().ToString(), animal, menu, timeLimit);
        AddTask(task);

        Debug.Log($"[CafeModel] New Customer: {animal.DisplayName} ({rarity}), Order: {menu.DisplayName}, Time: {timeLimit:F1}s");
    }

    private AnimalRarity GetRandomRarity()
    {
        // Normal 60%, Rare 30%, SuperRare 10%
        float roll = UnityEngine.Random.value;
        if (roll < 0.6f) return AnimalRarity.Normal;
        if (roll < 0.9f) return AnimalRarity.Rare;
        return AnimalRarity.SuperRare;
    }

    private AnimalData GetRandomAnimal(AnimalRarity rarity)
    {
        var allAnimals = _masterData.GetAllAnimals();

        // 指定レアリティのアニマルのみ抽出
        var candidates = allAnimals.Where(a => a.Rarity == rarity).ToList();

        // 指定レアリティのアニマルが居なければ、全アニマルから抽選 (フォールバック)
        if (candidates.Count == 0) candidates = allAnimals.ToList();
        if (candidates.Count == 0) return null;

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    private MenuItemData GetRandomMenu()
    {
        var allMenus = _masterData.GetAllMenuItems();
        if (allMenus.Count == 0) return null;
        return allMenus[UnityEngine.Random.Range(0, allMenus.Count)];
    }

    /// <summary>
    /// 指定スロットの参照を取得 (Presenter がイベント購読するため)
    /// </summary>
    public CookingSlot GetSlot(int index)
    {
        if (index < 0 || index >= _slots.Length) return null;
        return _slots[index];
    }

    /// <summary>
    /// 調理開始を要求する
    /// </summary>
    public void OrderItem(int slotIndex, MenuItemData itemData)
    {
        var slot = GetSlot(slotIndex);
        if (slot != null && !slot.IsBusy)
        {
            // 非同期処理開始 (Fire and Forget)
            // 完了時のコールバックはイベント経由で行うため await しない
            slot.StartCookingAsync(itemData, _cts.Token).Forget();
        }
    }

    /// <summary>
    /// 在庫を追加する
    /// </summary>
    public void AddStock(string itemId)
    {
        if (!_stockInventory.ContainsKey(itemId))
        {
            _stockInventory[itemId] = 0;
        }

        _stockInventory[itemId]++;

        // 通知
        OnStockChanged?.Invoke(itemId, _stockInventory[itemId]);
    }

    /// <summary>
    /// 在庫数取得メソッド
    /// </summary>
    public int GetStockCount(string itemId)
    {
        return _stockInventory.TryGetValue(itemId, out int count) ? count : 0;
    }

    /// <summary>
    /// タスク管理と完了判定メソッド
    /// </summary>
    public void AddTask(CustomerTask task)
    {
        _activeTasks[task.TaskId] = task;
        OnTaskAdded?.Invoke(task);
    }

    /// <summary>
    /// タスク完了を試みる
    /// </summary>
    /// <returns> 成功したら true </returns>
    public bool TryCompleteTask(string taskId)
    {
        // ゲーム終了時は操作無効
        if (!_isPlaying) return false;

        if (!_activeTasks.TryGetValue(taskId, out var task)) return false;

        string requiredItemId = task.OrderItem.ItemId;
        int currentStock = GetStockCount(requiredItemId);

        // 在庫チェック
        if (currentStock > 0)
        {
            _stockInventory[requiredItemId]--;
            OnStockChanged?.Invoke(requiredItemId, _stockInventory[requiredItemId]);

            // タスク削除
            _activeTasks.Remove(taskId);
            OnTaskRemoved?.Invoke(taskId);

            // タスク完了時にスコア加算 (現時点では固定値 100)
            AddScore(100);

            return true;
        }

        return false;
    }

    // スコア加算メソッド
    private void AddScore(int amount)
    {
        // ゲーム終了時はスコア加算しない
        if (!_isPlaying) return;

        _currentScore += amount;
        OnScoreChanged?.Invoke(_currentScore);
    }

    public IReadOnlyDictionary<string, CustomerTask> GetActiveTasks()
    {
        return _activeTasks;
    }

    public void Dispose()
    {
        _isPlaying = false;
        _cts.Cancel();
        _cts.Dispose();
    }
}
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
    // --- Constants (Magic Numbers) ---
    // Game Settings
    private const float k_GameDuration = 60.0f; // 1プレイ60秒
    private const float k_ExpConversionRate = 0.1f; // 経験値の変換レート (スコアの10%を経験値とする)

    // Spawning
    private const float k_SpawnWaitMin = 3.0f;
    private const float k_SpawnWaitMax = 5.0f;
    private const int k_BaseMaxCustomers = 5;
    private const int k_ScorePerCustomerDivisor = 500;

    // Rarity Thresholds
    private const float k_RarityNormalThreshold = 0.6f;
    private const float k_RarityRareThreshold = 0.9f;

    // Task Settings
    private const float k_TaskTimeLimitMin = 25.0f;
    private const float k_TaskTimeLimitMax = 35.0f;
    private const int k_ScorePerTaskCompletion = 100;

    // Score Ranks
    private const int k_RankScoreS = 1000;
    private const int k_RankScoreA = 700;
    private const int k_RankScoreB = 400;

    // Rank Bonus Rates
    private const float k_RankBonusS = 1.2f;
    private const float k_RankBonusA = 1.0f;
    private const float k_RankBonusB = 0.8f;
    private const float k_RankBonusC = 0.5f;

    // --- Events ---
    // Stock Update Event (ItemID, New stock quantity)
    public event Action<string, int> OnStockChanged;
    // Task increase/decrease events
    public event Action<CustomerTask> OnTaskAdded;
    public event Action<string> OnTaskRemoved;
    // Score Update Event
    public event Action<int> OnScoreChanged;
    // Game Time Related Events
    public event Action<float> OnGameTimeUpdated;
    public event Action<GameResult> OnGameTimeOver;

    // --- Fields ---
    // Slot
    private readonly CookingSlot[] _slots; // 本来は動的に増えるが、現時点では固定数2で実装
    // Inventory Data (ItemId -> Count)
    private readonly Dictionary<string, int> _stockInventory = new Dictionary<string, int>();
    // Active Task List (TaskID -> CustomerTask)
    private readonly Dictionary<string, CustomerTask> _activeTasks = new Dictionary<string, CustomerTask>();

    // Current Score
    private int _currentScore = 0;
    // Game Time State
    private float _gameRemainingTime;
    // Game Status
    private bool _isPlaying = false;
    // Token for canceling asynchronous processing
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    // Master Data Reference
    private readonly IMasterDataRepository _masterData;

    /// <summary>
    /// カフェ運営シーン (CafeOperation.unity) 起動時 (初期化時) に一度だけ実行するメソッド
    /// </summary>
    public CafeModel(IMasterDataRepository masterData)
    {
        _masterData = masterData;

        // スロット初期化
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

        _gameRemainingTime = k_GameDuration;
        OnGameTimeUpdated?.Invoke(_gameRemainingTime);

        SpawnLoopAsync(_cts.Token).Forget();
        TimeUpdateLoopAsync(_cts.Token).Forget();
    }

    /// <summary>
    /// 定期的にアニマルを来店させるループ
    /// </summary>
    private async UniTask SpawnLoopAsync(CancellationToken token)
    {
        while (_isPlaying && !token.IsCancellationRequested)
        {
            float waitTime = UnityEngine.Random.Range(3.0f, 5.0f);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);

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
                break;
            }

            expiredTaskIds.Clear();

            // 時間切れチェック
            foreach (var kvp in _activeTasks)
            {
                var task = kvp.Value;
                task.RemainingTime -= deltaTime;

                if (task.RemainingTime <= 0)
                {
                    expiredTaskIds.Add(task.TaskId);
                }
            }

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

        if (_currentScore >= k_RankScoreS)
        {
            rank = "S";
            bonusRate = k_RankBonusS;
        }
        else if (_currentScore >= k_RankScoreA)
        {
            rank = "A";
            bonusRate = k_RankBonusA;
        }
        else if (_currentScore >= k_RankScoreB)
        {
            rank = "B";
            bonusRate = k_RankBonusB;
        }
        else
        {
            rank = "C";
            bonusRate = k_RankBonusC;
        }

        // 報酬計算 (仮: スコア * クリア時ランク補正)
        int money = Mathf.FloorToInt(_currentScore * bonusRate);
        int exp = Mathf.FloorToInt(_currentScore * bonusRate * k_ExpConversionRate);

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
        }
    }

    /// <summary>
    /// 来店判定とタスク生成を行う
    /// </summary>
    private void TrySpawnCustomer()
    {
        // 最大来店数制限 (仮: ベース5体 + スコア/500)
        int maxCustomer = k_BaseMaxCustomers + (_currentScore / k_ScorePerCustomerDivisor);
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
        // 制限時間を設定してタスク生成
        float timeLimit = UnityEngine.Random.Range(k_TaskTimeLimitMin, k_TaskTimeLimitMax);
        var task = new CustomerTask(Guid.NewGuid().ToString(), animal, menu, timeLimit);
        AddTask(task);

        Debug.Log($"[CafeModel] New Customer: {animal.DisplayName} ({rarity}), Order: {menu.DisplayName}, Time: {timeLimit:F1}s");
    }

    /// <summary>
    /// レアリティ抽選処理
    /// </summary>
    private AnimalRarity GetRandomRarity()
    {
        // Normal 60%, Rare 30%, SuperRare 10%
        float roll = UnityEngine.Random.value;
        if (roll < k_RarityNormalThreshold) return AnimalRarity.Normal;
        if (roll < k_RarityRareThreshold) return AnimalRarity.Rare;
        return AnimalRarity.SuperRare;
    }

    /// <summary>
    /// 抽選されたレアリティ内のアニマル取得処理
    /// </summary>
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

    /// <summary>
    /// ランダムなメニュー取得処理
    /// </summary>
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

            _activeTasks.Remove(taskId);
            OnTaskRemoved?.Invoke(taskId);

            AddScore(k_ScorePerTaskCompletion);

            return true;
        }

        return false;
    }

    /// <summary>
    /// スコア加算メソッド
    /// </summary>
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
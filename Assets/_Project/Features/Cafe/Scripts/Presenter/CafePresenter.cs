using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using UnityEngine;

/// <summary>
/// CafeModel (ロジック) と ICafeView (表示) を仲介する Presenter
/// IStartable を 実装し、VContainer によって起動される
/// </summary>
public class CafePresenter : IStartable, IDisposable
{
    private readonly CafeModel _model;
    private readonly ICafeView _view;
    private readonly IMasterDataRepository _masterData;

    private int _currentSelectedSlotIndex = -1; // 現在選択中のスロット

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    // コンストラクタインジェクション (依存性注入)
    public CafePresenter(
        CafeModel model,
        ICafeView view,
        IMasterDataRepository masterData)
    {
        _model = model;
        _view = view;
        _masterData = masterData;
    }

    public void Start()
    {
        // --- View イベントの購読 ---
        _view.OnMenuSlotClicked += HandleSlotClicked;
        _view.OnMenuSelected += HandleMenuSelected;
        _view.OnTaskClicked += HandleTaskClicked;

        // --- Model イベントの購読 ---
        // 在庫変動
        _model.OnStockChanged += HandleStockChanged;
        _model.OnScoreChanged += HandleScoreChanged;
        _model.OnGameTimeUpdated += HandleGameTimeUpdated;
        _model.OnGameTimeOver += HandleGameTimeOver;

        // Model のタスク変動を View に反映
        _model.OnTaskAdded += (task) =>
        {
            _view.AddCustomerTask(task);
            CheckTaskCompletable(task); // 追加直後にも在庫チェック
        };
        _model.OnTaskRemoved += (task) => _view.RemoveCustomerTask(task);

        // アプリ開始時、マスターデータを View に渡して在庫リスト枠を作成させる
        var allItems = _masterData.GetAllMenuItems();
        _view.InitializeStockList(allItems);

        // ゲームループ (自動来店) を開始
        _model.StartGameLoop();

        // UI タイマー同期ループを開始
        SyncTimeLoopAsync(_cts.Token).Forget();

        // スロット状態変化 (スロット 0, 1 を監視)
        // ※本来は動的生成だが現時点では固定
        for (int i = 0; i < 2; i++)
        {
            int index = i; // クロージャキャプチャ用
            var slot = _model.GetSlot(i);

            // 開始 / 終了 の見た目同期
            slot.OnStateChanged += (isBusy) => _view.UpdateSlotState(index, isBusy);

            // 調理完了時の在庫加算
            slot.OnCookingCompleted += (itemId) => _model.AddStock(itemId);
        }
    }

    /// <summary>
    /// 定期的に Model の各タスク残り時間を取得し、View を更新する
    /// </summary>
    private async UniTask SyncTimeLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            // 毎フレーム更新
            await UniTask.Yield(PlayerLoopTiming.Update, token);

            // 現在のアクティブなタスク一覧を取得
            var tasks = _model.GetActiveTasks();

            // 各タスクの時間を View に反映
            foreach (var kvp in tasks)
            {
                var task = kvp.Value;
                _view.UpdateTaskTime(task.TaskId, task.RemainingTime);
            }
        }
    }

    // スロットがクリックされた
    private void HandleSlotClicked(int slotIndex)
    {
        _currentSelectedSlotIndex = slotIndex;

        // マスターデータから全メニューを取得し、View に渡して表示させる
        var allMenus = _masterData.GetAllMenuItems();
        _view.ShowMenuSelection(allMenus);
    }

    // メニューが選択された (作成開始)
    private void HandleMenuSelected(string itemId)
    {
        if (_currentSelectedSlotIndex == -1) return;

        // マスタデータ取得
        var itemData = _masterData.GetMenuItemById(itemId);
        if (itemData != null)
        {
            // Model へ調理開始を命令
            _model.OrderItem(_currentSelectedSlotIndex, itemData);
        }

        // 選択状態解除
        _currentSelectedSlotIndex = -1;
    }

    // タスクバーがクリックされた (完了試行)
    private void HandleTaskClicked(string taskId)
    {
        // Model に完了を依頼
        bool success = _model.TryCompleteTask(taskId);

        if (success)
        {
            Debug.Log($"[CafePresenter] タスク完了！ ID: {taskId}");

            // 在庫が減ったので、残りのタスクの完了可否を再チェック
            RefreshAllTasksCompletable();
        }
    }

    // 在庫が変動した
    private void HandleStockChanged(string itemId, int count)
    {
        _view.UpdateStockDisplay(itemId, count);

        // 在庫が変わったので、全タスクの完了可否を再チェック
        RefreshAllTasksCompletable();
    }

    private void HandleScoreChanged(int newScore)
    {
        _view.UpdateScoreDisplay(newScore);
    }

    private void HandleGameTimeUpdated(float remainingTime)
    {
        _view.UpdateGameTime(remainingTime);
    }

    private void HandleGameTimeOver(GameResult result)
    {
        Debug.Log($"[CafePresenter] Game Over! Score = {result.Score}, Rank: {result.Rank}");
        _view.ShowResultPopup(result);
    }

    // タスク完了可否のチェックロジック
    // 全タスクの状態更新 (LINQ なし)
    private void RefreshAllTasksCompletable()
    {
        var tasks = _model.GetActiveTasks();
        foreach (var kvp in tasks)
        {
            CheckTaskCompletable(kvp.Value);
        }
    }

    // 個別タスクの状態チェック
    private void CheckTaskCompletable(CustomerTask task)
    {
        int stock = _model.GetStockCount(task.OrderItem.ItemId);
        bool isCompletable = stock > 0;

        // View に通知
        _view.UpdateTaskCompletable(task.TaskId, isCompletable);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();

        // イベント購読解除 (省略可ではあるがマナーとして)
        _view.OnMenuSlotClicked -= HandleSlotClicked;
        _view.OnMenuSelected -= HandleMenuSelected;
        _view.OnTaskClicked -= HandleTaskClicked;

        _model.OnStockChanged -= HandleStockChanged;
        _model.OnScoreChanged -= HandleScoreChanged;
        _model.OnGameTimeUpdated -= HandleGameTimeUpdated;
        _model.OnGameTimeOver -= HandleGameTimeOver;

        // CafeModel は VContainer が管理しているため
        // VContainer が自動的に Dispose を呼びます
        // そのため Presenter が呼ぶ必要はありません
    }
}

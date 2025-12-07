using System;
using System.Collections.Generic;
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

    // スロット (本来は動的に増えるが、現時点では固定数2で実装)
    private readonly CookingSlot[] _slots;

    //  在庫データ (ItemId -> Count)
    private readonly Dictionary<string, int> _stockInventory = new Dictionary<string, int>();

    // アクティブなタスクリスト (TaskID -> CustomerTask)
    private readonly Dictionary<string, CustomerTask> _activeTasks = new Dictionary<string, CustomerTask>();

    // 非同期処理キャンセル用トークン
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    // カフェ運営シーン (CafeOperation.unity) 起動時 (初期化時) に一度だけ実行するメソッド
    public CafeModel()
    {
        // スロット初期化 (2つ)
        _slots = new CookingSlot[2];
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i] = new CookingSlot();
        }
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

            return true;
        }

        return false;
    }

    public IReadOnlyDictionary<string, CustomerTask> GetActiveTasks()
    {
        return _activeTasks;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}

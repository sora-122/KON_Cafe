using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// カフェ運営のデータとビジネスロジックを保持する Model
/// スロット管理と在庫管理を行う
/// </summary>
public class CafeModel : IDisposable
{
    // 在庫更新イベント (ItemID, 新しい在庫数)
    public event Action<string, int> OnStockChanged;

    // スロット (本来は動的に増えるが、現時点では固定数2で実装)
    private readonly CookingSlot[] _slots;

    //  在庫データ (ItemId -> Count)
    private readonly Dictionary<string, int> _stockInventory = new Dictionary<string, int>();

    // 非同期処理キャンセル用
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

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}

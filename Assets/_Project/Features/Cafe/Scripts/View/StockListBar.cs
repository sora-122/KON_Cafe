using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全メニューの在庫状況を一覧表示する UI バー
/// 起動時に全メニュー分スロットを生成し、ID で高速に更新できるように管理する
/// </summary>
public class StockListBar : MonoBehaviour
{
    [SerializeField] private StockItemElement _elementPrefab;
    [SerializeField] private Transform _container;

    // ID -> UI 要素 のマッピング (検索用)
    private readonly Dictionary<string, StockItemElement> _elements = new Dictionary<string, StockItemElement>();

    /// <summary>
    /// マスターデータを基にリストを初期構築する
    /// </summary>
    public void Initialize(IReadOnlyList<MenuItemData> allItems)
    {
        // 既存の要素があればクリア (本来は再利用すべきだが、初期化は1回のみの前提)
        foreach (Transform child in _container)
        {
            Destroy(child.gameObject);
        }
        _elements.Clear();

        // LINQ を使わず foreach で生成
        foreach (var item in allItems)
        {
            var element = Instantiate(_elementPrefab, _container);
            element.Initialize(item.Icon);

            // Dictionary に登録して検索可能にする
            _elements[item.ItemId] = element;
        }
    }

    /// <summary>
    /// 指定アイテムの在庫表示を更新する
    /// </summary>
    public void UpdateStock(string itemId, int newCount)
    {
        if (_elements.TryGetValue(itemId, out var element))
        {
            element.UpdateCount(newCount);
        }
        else
        {
            Debug.LogWarning($"[StockListBar] 未登録の ItemID が指定されました: {itemId}");
        }
    }
}

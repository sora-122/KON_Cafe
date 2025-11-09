using System.Collections.Generic;
using System.Linq;
using UnityEngine; // Debug,LogWarning/Error のため

/// <summary>
/// IMasterDataRepository の実装クラス
/// </summary>
public class MasterDataRepository : IMasterDataRepository
{
    // Unity C# コーディング規約に従い、private フィールドは _camelCase
    private readonly Dictionary<string, MenuItemData> _itemDictionary;

    // VContainer から MenuItemMaster (SO) を注入してもらう
    public MasterDataRepository(MenuItemMaster master)
    {
        _itemDictionary = master.Items
            .GroupBy(item => item.ItemId)
            .ToDictionary(group => group.Key, group => group.First());

        if (_itemDictionary.Count != master.Items.Count)
        {
            Debug.LogWarning("[MasterDataRepository] メニューアイテム ID に重複があります。MenuItemMaster を確認してください。");
        }
    }

    public MenuItemData GetMenuItemById(string itemId)
    {
        if (_itemDictionary.TryGetValue(itemId, out var data))
        {
            return data;
        }

        Debug.LogError($"[MasterDataRepository] Item ID: {itemId} が見つかりません。");
        return null;
    }

    public IReadOnlyList<MenuItemData> GetAllMenuItems()
    {
        return _itemDictionary.Values.ToList().AsReadOnly();
    }
}


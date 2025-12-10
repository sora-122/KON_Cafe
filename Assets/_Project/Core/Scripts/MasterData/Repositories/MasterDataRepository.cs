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
    private readonly Dictionary<string, AnimalData> _animalDictionary;

    // VContainer から MenuItemMaster (SO) を注入してもらう
    public MasterDataRepository(MenuItemMaster itemMaster, AnimalMaster animalMaster)
    {
        // --- 1．メニューデータの構築 ---
        // GroupBy で重複を排除して Dictionary に変換し、安全に処理する
        _itemDictionary = itemMaster.Items
            .GroupBy(item => item.ItemId)
            .ToDictionary(group => group.Key, group => group.First());

        // 重複があった場合は警告を出す
        if (_itemDictionary.Count != itemMaster.Items.Count)
        {
            Debug.LogWarning("[MasterDataRepository] メニューアイテム ID に重複があります。MenuItemMaster を確認してください。");
        }

        // --- 2．アニマルデータの構築 ---
        // アニマルデータも同様に GroupBy で重複を排除して Dictionary に変換し、安全に処理する
        _animalDictionary = animalMaster.Animals
            .GroupBy(animal => animal.Id)
            .ToDictionary(group => group.Key, group => group.First());

        // アニマルの重複チェック警告
        if (_animalDictionary.Count != animalMaster.Animals.Count)
        {
            Debug.LogWarning("[MasterDataRepository] アニマル ID に重複があります。AnimalMaster を確認してください。");
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

    public AnimalData GetAnimalById(string id)
    {
        if (_animalDictionary.TryGetValue(id, out var data))
        {
            return data;
        }
        Debug.LogError($"[MasterDataRepository] Animal ID: {id} が見つかりません。");
        return null;
    }

    public IReadOnlyList<MenuItemData> GetAllMenuItems()
    {
        return _itemDictionary.Values.ToList().AsReadOnly();
    }

    public IReadOnlyList<AnimalData> GetAllAnimals()
    {
        return _animalDictionary.Values.ToList().AsReadOnly();
    }
}


using System.Collections.Generic;

/// <summary>
/// マスターデータへのアクセスを提供するインターフェース
/// </summary>
public interface IMasterDataRepository
{
    /// <summary>
    /// ID を指定してメニューデータを取得する
    /// </summary>
    MenuItemData GetMenuItemById(string itemId);

    /// <summary>
    /// ID を指定してアニマルデータを取得する
    /// </summary>
    AnimalData GetAnimalById(string id);

    /// <summary>
    /// 全てのメニューデータを取得する
    /// </summary>
    IReadOnlyList<MenuItemData> GetAllMenuItems();

    /// <summary>
    /// 全アニマルデータを取得する
    /// </summary>
    IReadOnlyList<AnimalData> GetAllAnimals();
}

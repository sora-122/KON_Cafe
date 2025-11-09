using System.Collections.Generic;

/// <summary>
/// マスターデータへのアクセスを提供するインターフェース
/// </summary>
public interface IMasterDataRepository
{
    /// <summary>
    /// AC2: ID を指定してメニューデータを取得する
    /// </summary>
    MenuItemData GetMenuItemById(string itemId);

    /// <summary>
    /// 全てのメニューデータを取得する
    /// </summary>
    IReadOnlyList<MenuItemData> GetAllMenuItems();
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全ての MenuItemData アセットを保持するデータベース
/// </summary>
[CreateAssetMenu(fileName = "MenuItemMaster", menuName = "KONCafe/Master/MenuItemMaster")]
public class MenuItemMaster : ScriptableObject
{
    [SerializeField]
    private List<MenuItemData> _items;

    // 外部には読み取り専用リストとして公開する
    public IReadOnlyList<MenuItemData> Items => _items;
}

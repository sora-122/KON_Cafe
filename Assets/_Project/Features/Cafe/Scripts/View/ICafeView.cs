using System;
using System.Collections.Generic;

/// <summary>
/// カフェ運営画面 (View) が
/// Presenter に対して公開するインターフェース
/// </summary>
public interface ICafeView
{
    /// <summary>
    /// いずれかのメニュー作成スロットがクリックされた際に発生するイベント
    /// (引数はスロットのインデックス)
    /// </summary>
    event Action<int> OnMenuSlotClicked;

    /// <summary>
    /// メニュー選択バーでメニューが選ばれたときのイベント
    /// (引数はメニュー ID )
    /// </summary>
    event Action<string> OnMenuSelected;

    /// <summary>
    /// 指定スロットの見た目状態を更新する
    /// </summary>
    void UpdateSlotState(int slotIndex, bool isCooking);

    /// <summary>
    /// 在庫数表示を更新する
    /// </summary>
    void UpdateStockDisplay(string itemId, int newCount);

    /// <summary>
    /// メニュー選択バーを表示する
    /// </summary>
    void ShowMenuSelection(IReadOnlyList<MenuItemData> menuItems);
}

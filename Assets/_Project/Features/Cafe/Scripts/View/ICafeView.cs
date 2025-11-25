using System;

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
}

using System;
using VContainer;
using VContainer.Unity;
using UnityEngine;

/// <summary>
/// CafeModel (ロジック) と ICafeView (表示) を仲介する Presenter
/// IStartable を 実装し、VContainer によって起動される
/// </summary>
public class CafePresenter : IStartable, IDisposable
{
    private readonly CafeModel _model;
    private readonly ICafeView _view;
    private readonly IMasterDataRepository _masterData;

    private int _currentSelectedSlotIndex = -1; // 現在選択中のスロット

    // コンストラクタインジェクション (依存性注入)
    public CafePresenter(
        CafeModel model,
        ICafeView view,
        IMasterDataRepository masterData)
    {
        _model = model;
        _view = view;
        _masterData = masterData;
    }

    public void Start()
    {
        // --- View イベントの購読 ---
        _view.OnMenuSlotClicked += HandleSlotClicked;
        _view.OnMenuSelected += HandleMenuSelected;

        // --- Model イベントの購読 ---
        // 在庫変動
        _model.OnStockChanged += HandleStockChanged;

        // アプリ開始時、マスターデータを View に渡して在庫リスト枠を作成させる
        var allItems = _masterData.GetAllMenuItems();
        _view.InitializeStockList(allItems);

        // スロット状態変化 (スロット 0, 1 を監視)
        // ※本来は動的生成だが現時点では固定
        for (int i = 0; i < 2; i++)
        {
            int index = i; // クロージャキャプチャ用
            var slot = _model.GetSlot(i);

            // 開始 / 終了 の見た目同期
            slot.OnStateChanged += (isBusy) => _view.UpdateSlotState(index, isBusy);

            // 調理完了時の在庫加算
            slot.OnCookingCompleted += (itemId) => _model.AddStock(itemId);
        }
    }

    // スロットがクリックされた
    private void HandleSlotClicked(int slotIndex)
    {
        _currentSelectedSlotIndex = slotIndex;

        // マスターデータから全メニューを取得し、View に渡して表示させる
        var allMenus = _masterData.GetAllMenuItems();
        _view.ShowMenuSelection(allMenus);
    }

    // メニューが選択された (作成開始)
    private void HandleMenuSelected(string itemId)
    {
        if (_currentSelectedSlotIndex == -1) return;

        // マスタデータ取得
        var itemData = _masterData.GetMenuItemById(itemId);
        if (itemData != null)
        {
            // Model へ調理開始を命令
            _model.OrderItem(_currentSelectedSlotIndex, itemData);
        }

        // 選択状態解除
        _currentSelectedSlotIndex = -1;
    }

    // 在庫が変動した
    private void HandleStockChanged(string itemId, int count)
    {
        _view.UpdateStockDisplay(itemId, count);
    }

    public void Dispose()
    {
        // イベント購読解除 (省略可ではあるがマナーとして)
        _view.OnMenuSlotClicked -= HandleSlotClicked;
        _view.OnMenuSelected -= HandleMenuSelected;

        _model.OnStockChanged -= HandleStockChanged;

        // CafeModel は VContainer が管理しているため
        // VContainer が自動的に Dispose を呼びます
        // そのため Presenter が呼ぶ必要はありません
    }
}

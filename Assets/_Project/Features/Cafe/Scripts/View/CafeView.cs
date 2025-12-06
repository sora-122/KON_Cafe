using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カフェ運営画面 (View) の実装クラス (MonoBehaviour)
/// ICafeView インターフェースを実装する
/// </summary>
public class CafeView : MonoBehaviour, ICafeView
{
    [Header("UI仕様")]
    [Tooltip("仕様: ステータスバーのエリア")]
    [SerializeField]
    private GameObject _statusBarArea;

    [Tooltip("仕様: タスクバーが表示されるエリア")]
    [SerializeField]
    private GameObject _taskBarArea;

    [Tooltip("仕様: メニュー作成スロットの配置エリア")]
    [SerializeField]
    private Transform _menuSlotsArea;

    [Header("Prefab参照")]
    [SerializeField]
    private MenuSlot _slotPrefab;

    [Header("UI Components")]
    [SerializeField]
    private MenuSelectionBar _menuSelectionBar; // プレハブではなくシーン配置 or 生成されたインスタンス
    [SerializeField]
    private StockListBar _stockListBar;
    [SerializeField]
    private TaskListPanel _taskListPanel;


    // View が管理するスロットのリスト
    private readonly List<MenuSlot> _instantiatedSlots = new List<MenuSlot>();


    // --- ICafeView の実装 ---

    /// <summary>
    /// Presenter が購読するスロットクリックイベント
    /// </summary>
    public event Action<int> OnMenuSlotClicked;

    public event Action<string> OnMenuSelected;

    public void UpdateSlotState(int slotIndex, bool isCooking)
    {
        if (slotIndex >= 0 && slotIndex < _instantiatedSlots.Count)
        {
            _instantiatedSlots[slotIndex].SetCookingState(isCooking);
        }
    }

    public void InitializeStockList(IReadOnlyList<MenuItemData> allItems)
    {
        if (_stockListBar != null)
        {
            _stockListBar.Initialize(allItems);
        }
    }

    public void UpdateStockDisplay(string itemId, int newCount)
    {
        // ストック数の増加確認ログ
        Debug.Log($"[CafeView] 在庫更新: {itemId} = {newCount}個");

        if (_stockListBar != null)
        {
            _stockListBar.UpdateStock(itemId, newCount);
        }
    }

    public void AddCustomerTask(CustomerTask task)
    {
        if (_taskListPanel != null)
        {
            _taskListPanel.AddTask(task);
        }
    }


    // --- Unity ライフサイクル ---

    /// <summary>
    /// 検証のため、ダミーのスロットを生成する
    /// </summary>
    private void Start()
    {
        // 初期化: バーを閉じておく
        if (_menuSelectionBar != null)
        {
            _menuSelectionBar.Close();
            _menuSelectionBar.OnMenuSelected += HandleMenuSelected;
        }

        // TODO: 将来のタスクで、プレイヤーやアニマルのデータ (Model) に基づいて生成する
        SpawnSlot(0); // ダミー: プレイヤースロット
        SpawnSlot(1); // ダミー: お手伝いアニマル1
    }

    /// <summary>
    /// スロットを生成し、イベントを購読する
    /// </summary>
    /// <param name="slotIndex"></param>
    private void SpawnSlot(int slotIndex)
    {
        // _menuSlotArea を親として _slotPrefab からインスタンスを生成する
        MenuSlot newSlot = Instantiate(_slotPrefab, _menuSlotsArea);

        // スロットがクリックされたら、HandleSlotClicked メソッドを呼ぶ
        newSlot.OnClicked += HandleSlotClicked;

        _instantiatedSlots.Add(newSlot);
    }

    /// <summary>
    /// MenuSlot.OnClicked イベントを処理する
    /// </summary>
    private void HandleSlotClicked(MenuSlot clickedSlot)
    {
        // クリックされたスロットがリストの何番目か (＝インデックス) を特定する
        int index = _instantiatedSlots.IndexOf(clickedSlot);

        // --- 受入基準の達成 ---
        // スロットをタップすると、コンソールログ等でタップイベントが確認できる
        Debug.Log($"[CafeView] メニュースロット {index} がタップされました。");

        // Presenter (購読者がいれば) インデックスを通知
        OnMenuSlotClicked?.Invoke(index);
    }

    public void ShowMenuSelection(IReadOnlyList<MenuItemData> menuItems)
    {
        if (_menuSelectionBar != null)
        {
            _menuSelectionBar.Open(menuItems);
        }
    }

    // バーでメニューが選ばれた時の挙動
    private void HandleMenuSelected(string menuId)
    {
        // ID ログ確認 & 閉じる
        Debug.Log($"[CafeView] Menu Selected: ID = {menuId}");

        _menuSelectionBar.Close();

        OnMenuSelected?.Invoke(menuId);
    }
}

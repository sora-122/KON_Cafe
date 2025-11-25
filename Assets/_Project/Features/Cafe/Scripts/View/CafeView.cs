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

    // View が管理するスロットのリスト
    private readonly List<MenuSlot> _instantiatedSlots = new List<MenuSlot>();


    // --- ICafeView の実装 ---

    /// <summary>
    /// Presenter が購読するスロットクリックイベント
    /// </summary>
    public event Action<int> OnMenuSlotClicked;


    // --- Unity ライフサイクル ---

    /// <summary>
    /// 検証のため、ダミーのスロットを生成する
    /// </summary>
    private void Start()
    {
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
}

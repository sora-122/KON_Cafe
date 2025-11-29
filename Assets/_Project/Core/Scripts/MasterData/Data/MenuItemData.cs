using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lean GDD に基づくメニューデータの分類
/// </summary>]
public enum MenuItemCategory
{
    Beverage, // 飲料
    Dessert,  // デザート
    Dinner,   // ディナー
}

/// <summary>
/// メニューアイテムのパラメータを定義するマスターデータ (ScriptableObject)
/// LeanGDD と Trelloカード の仕様に基づく
/// </summary>
[CreateAssetMenu(fileName = "MenuItem_", menuName = "KONCafe/Master/Menu Item Data")]
public class MenuItemData : ScriptableObject
{
    /// Unity C# コーディング規約に従い、[SerializeField] private + アンダースコア
    [Header("基本情報")]
    [SerializeField, Tooltip("データを参照するためのユニークID (例: 'coffee')。AC2 で使用")]
    private string _itemId;

    [SerializeField, Tooltip("Inspectorでの表示名 (例: 'コーヒー')")]
    private string _displayName;

    [SerializeField, Tooltip("メニューのアイコン画像")]
    private Sprite _icon;

    [SerializeField, Tooltip("メニューの分類")]
    private MenuItemCategory _category;

    [Header("カフェ運営パラメータ")]
    [SerializeField, Tooltip("GDD準拠: メニュー作成スロットの使用数")]
    private int _slotsUsed = 1;

    [SerializeField, Tooltip("GDD準拠: 作成所要時間(秒)")]
    private float _creationTimeSeconds;

    // Unity C# コーディング規約に従い、Publicプロパティは PascalCase で命名
    public string ItemId => _itemId;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public MenuItemCategory Category => _category;
    public int SlotsUsed => _slotsUsed;
    public float CreationTimeSeconds => _creationTimeSeconds;

    // --- バリデーターの先取り実装 ---
    // Unity エディタ上で値を変更した瞬間にチェックが走る
    private void OnValidate()
    {
        if (_icon == null)
        {
            // アイコン未設定を警告
            Debug.LogWarning($"[MenuItemData] Item '{_itemId}' にアイコンが設定されていません！", this);
        }
    }
}
using UnityEngine;

// レアリティ定義
public enum AnimalRarity
{
    Normal,
    Rare,
    SuperRare
}

/// <summary>
/// 来店するアニマルの基本情報を定義するマスターデータ
/// </summary>
[CreateAssetMenu(fileName = "Animal_", menuName = "KONCafe/Master/Animal Data")]
public class AnimalData : ScriptableObject
{
    [SerializeField, Tooltip("アニマルID (例: 'fox')")]
    private string _id;

    [SerializeField, Tooltip("表示名 (例: 'きつね')")]
    private string _displayName;

    [SerializeField, Tooltip("アニマルのアイコン画像")]
    private Sprite _icon;

    [SerializeField, Tooltip("出現レアリティ")]
    private AnimalRarity _rarity;

    // 読み取り専用プロパティ
    public string Id => _id;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public AnimalRarity Rarity => _rarity;
}

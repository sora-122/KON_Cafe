using UnityEngine;

/// <summary>
/// ランクごとのデータを保持するシリアライズ可能な構造体
/// (ScriptableObject の Inspector に表示するため [System.Serializable] が必要)
/// </summary>
[System.Serializable]
public struct RankData
{
    [SerializeField, Tooltip("ランク (Lv) ")]
    private int _rank;

    [SerializeField, Tooltip("次のランクまでに必要な累計経験値 (GDDのテーブルに基づく)")]
    private int _totalExpToNextRank;

    [SerializeField, Tooltip("Trello仕様: このランクで適用されるボーナス (例: 来客数)")]
    private int _visitorCountBonus;

    // 読み取り専用プロパティ (規約)
    public int Rank => _rank;
    public int TotalExpToNextRank => _totalExpToNextRank;
    public int VisitorCountBonus => _visitorCountBonus;
}

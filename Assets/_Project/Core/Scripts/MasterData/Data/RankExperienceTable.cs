using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーランクごとの経験値テーブル
/// </summary>
[CreateAssetMenu(fileName = "RankExperienceTable", menuName = "KONCafe/Master/RankExperienceTable")]
public class RankExperienceTable : ScriptableObject
{
    [SerializeField, Tooltip("ランクデータを昇順で設定してください")]
    private List<RankData> _rankDataList;

    public IReadOnlyList<RankData> RankDataList => _rankDataList;
}

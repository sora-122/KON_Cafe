using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// IRankDataRepository の実装クラス (POCO)
/// </summary>
public class RankDataRepository : IRankDataRepository
{
    private readonly Dictionary<int, RankData> _rankDictionary;
    private readonly int _maxRank;

    // VContainer から RankExperienceTable (SO) を注入してもらう
    public RankDataRepository(RankExperienceTable table)
    {
        // List を Dictionary に変換して高速な参照を可能にする
        _rankDictionary = table.RankDataList.ToDictionary(data => data.Rank);
        _maxRank = table.RankDataList.Max(data => data.Rank);
    }

    public RankData GetRankData(int rank)
    {
        if (_rankDictionary.TryGetValue(rank, out var data))
        {
            return data;
        }

        Debug.LogError($"[RankDataRepository] ランク: {rank} のデータが見つかりません。");
        // 安全のため、見つからない場合は Rank 1 のデータを返す (または例外)
        return _rankDictionary.TryGetValue(1, out var defaultData)
            ? defaultData
            : default(RankData);
    }

    public int GetMaxRank()
    {
        return _maxRank;
    }
}

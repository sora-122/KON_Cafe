/// <summary>
/// ランクデータへのアクセスを提供するインターフェース
/// </summary>
public interface IRankDataRepository
{
    /// <summary>
    /// 指定したランクのデータを取得する
    /// </summary>
    RankData GetRankData(int rank);

    /// <summary>
    /// マスターデータに定義されている最大ランクを取得する
    /// </summary>
    int GetMaxRank();
}

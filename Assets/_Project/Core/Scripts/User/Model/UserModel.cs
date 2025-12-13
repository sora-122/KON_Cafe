using UnityEngine;

/// <summary>
/// ユーザーデータ (所持金・経験値など) に関するビジネスロジックを扱う Model
/// </summary>
public class UserModel
{
    private readonly IPlayerDataRepository _playerRepository;
    private readonly IRankDataRepository _rankRepository;

    // コンストラクタで依存性注入
    public UserModel(
        IPlayerDataRepository playerRepository,
        IRankDataRepository rankRepository
        )
    {
        _playerRepository = playerRepository;
        _rankRepository = rankRepository;
    }

    /// <summary>
    /// ゲーム結果 (報酬) をユーザーデータに適用して保存する
    /// </summary>
    public void ApplyGameResult(GameResult result)
    {
        // 1. ロード
        var data = _playerRepository.Load();

        // 2. 計算 (ビジネスロック)
        data.Money += result.Money;
        data.Experience += result.Experience;

        // ランクアップ判定ループ
        // 一気に大量の経験値を得て、複数回ランクアップする場合に対応するため while を使用
        while (true)
        {
            // 最大ランクチェック
            int maxRank = _rankRepository.GetMaxRank();
            if (data.Rank >= maxRank)
            {
                // カンスト時は経験値を0にする、あるいは上限で止めるなどの仕様にする
                // 今回は「必要経験値を引く処理」ができなくなるためループを抜ける
                break;
            }

            // 現在のランクに必要な経験値を取得
            // RankData は struct なので null チェックは不要
            var rankData = _rankRepository.GetRankData(data.Rank);

            // 安全策: 必要経験値が 0 以下の場合は設定ミス等の可能性があるため、無限ループ防止で抜ける
            if (rankData.TotalExpToNextRank <= 0)
            {
                Debug.LogWarning($"[UserModel] Rank {data.Rank} required exp is <= 0. Loop break.");
                break;
            }

            // ランクアップ条件を満たしているか？
                if (data.Experience >= rankData.TotalExpToNextRank)
                {
                    data.Experience -= rankData.TotalExpToNextRank;
                    data.Rank++;

                    Debug.Log($"[UserModel] Rank Up! {data.Rank - 1} -> {data.Rank}");
                }
                else
                {
                    // 満たしていないなら終了
                    break;
                }
        }

        // 3. 保存
        _playerRepository.Save(data);

        Debug.Log($"[UserModel] Result Applied: Rank: {data.Rank}, Total Money: {data.Money}, Total Exp: {data.Experience}");
    }

    /// <summary>
    /// 現在のデータを取得する (読み取り専用)
    /// </summary>
    public PlayerData GetData()
    {
        return _playerRepository.Load();
    }
}

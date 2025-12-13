using UnityEngine;

/// <summary>
/// ユーザーデータ (所持金・経験値など) に関するビジネスロジックを扱う Model
/// </summary>
public class UserModel
{
    private readonly IPlayerDataRepository _repository;

    public UserModel(IPlayerDataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// ゲーム結果 (報酬) をユーザーデータに適用して保存する
    /// </summary>
    public void ApplyGameResult(GameResult result)
    {
        // 1. ロード
        var data = _repository.Load();

        // 2. 計算 (ビジネスロック)
        data.Money += result.Money;
        data.Experience += result.Experience;

        // 将来的に「レベルアップ判定」や「アイテム付与」などもここで行う

        // 3. 保存
        _repository.Save(data);

        Debug.Log($"[UserModel] Result Applied: Total Money: {data.Money}, Total Exp: {data.Experience}");
    }

    /// <summary>
    /// 現在のデータを取得する (読み取り専用)
    /// </summary>
    public PlayerData GetData()
    {
        return _repository.Load();
    }
}

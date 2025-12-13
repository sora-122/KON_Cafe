public interface IPlayerDataRepository
{
    /// <summary>
    /// 保存されたプレイヤーデータを読み込む
    /// データが無い場合は初期値を返す
    /// </summary>
    PlayerData Load();

    /// <summary>
    /// プレイヤーデータを保存する
    /// </summary>
    void Save(PlayerData data);
}
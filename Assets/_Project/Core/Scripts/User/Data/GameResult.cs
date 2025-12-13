using System;

/// <summary>
/// ゲームプレイの結果データ
/// Model (Feature) や UserModel (Core) もアクセス出来るよう、Core 層に配置
/// </summary>
[Serializable]
public class GameResult
{
    public int Score;
    public string Rank;
    public int Money;
    public int Experience;
}
using System;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの永続データを管理するクラス
/// JsonUtility でシリアライズするため Serializable 属性を付与
/// </summary>
[Serializable]
public class PlayerData
{
    public int Rank;
    public int Money;
    public int Experience; // 現在のランクにおける蓄積経験値

    // 所持アイテムの ID リスト (将来用)
    public List<string> OwnedItemIds = new List<string>();

    // コンストラクタ (初期値設定)
    public PlayerData()
    {
        Rank = 1;
        Money = 0;
        Experience = 0;
    }
}

using UnityEngine;

public class PlayerDataRepository : IPlayerDataRepository
{
    private const string k_SaveKey = "KONCafe_PlayerData";

    public PlayerData Load()
    {
        if (PlayerPrefs.HasKey(k_SaveKey))
        {
            string json = PlayerPrefs.GetString(k_SaveKey);

            // JSON からオブジェクトに復元
            return JsonUtility.FromJson<PlayerData>(json);
        }

        // データがない場合は新規作成して返す
        return new PlayerData();
    }

    public void Save(PlayerData data)
    {
        // オブジェクトを JSON 文字列に変換
        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(k_SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log($"[PlayerDataRepository] Saved: {json}");
    }
}
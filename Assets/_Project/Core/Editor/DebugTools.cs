#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class DebugTools
{
    // Unity の上部メニューに "KONCafe" > "Result Save Data" を追加
    [MenuItem("KONCafe/Debug/Result Save Data")]
    public static void ResetSaveData()
    {
        // PlayerDataRepository で定義したキーを指定して削除
        PlayerPrefs.DeleteKey("KONCafe_PlayerData");
        PlayerPrefs.Save();

        Debug.Log("Delete Save Data (Key: KONCafe_PlayerData)");
    }
}
#endif

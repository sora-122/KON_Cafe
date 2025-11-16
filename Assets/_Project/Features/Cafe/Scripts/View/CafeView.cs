using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カフェ運営画面 (View) の実装クラス (MonoBehaviour)
/// ICafeView インターフェースを実装する
/// </summary>
public class CafeView : MonoBehaviour, ICafeView
{
    [Header("UI仕様")]
    [Tooltip("仕様: ステータスバーのエリア")]
    [SerializeField]
    private GameObject _statusBarArea;

    [Tooltip("仕様: タスクバーが表示されるエリア")]
    [SerializeField]
    private GameObject _taskBarArea;

    [Tooltip("仕様: メニュー作成スロットの配置エリア")]
    [SerializeField]
    private GameObject _menuSlotsArea;

    // TODO: 将来のタスクで ICafeView のメソッドを実装していく
    // (例)
    // public void UpdateScore(int score)
    // {
    //      _scoreText.text = score.ToString();
    // }
}

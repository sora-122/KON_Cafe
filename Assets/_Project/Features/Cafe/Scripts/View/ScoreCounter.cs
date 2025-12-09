using UnityEngine;
using TMPro;

/// <summary>
/// スコアの数値を表示する View コンポーネント
/// </summary>
public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    /// <summary>
    /// スコア表示を更新する
    /// </summary>
    public void UpdateScore(int score)
    {
        if (_scoreText != null)
        {
            _scoreText.text = score.ToString();
        }
    }
}

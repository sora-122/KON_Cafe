using UnityEngine;
using TMPro;

/// <summary>
/// ゲーム全体の残り時間を表示する View コンポーネント
/// </summary>
public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    /// <summary>
    /// 時間表示を更新する
    /// </summary>
    /// <param name="remainingSeconds"> 残り時間 </param>
    public void UpdateTime(float remainingSeconds)
    {
        if (_timerText != null)
        {
            // 整数で表示
            // 0未満にはならないように Clamp
            float displayTime = Mathf.Max(0, remainingSeconds);
            _timerText.text = Mathf.CeilToInt(displayTime).ToString();

            // 残り10秒でテキストカラーを赤色にする
            if (displayTime <= 10.0f)
            {
                _timerText.color = Color.red;
            }
            else
            {
                _timerText.color = Color.black;
            }
        }
    }
}
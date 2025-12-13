using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPopup : MonoBehaviour
{
    public event Action OnRetryClicked; // 現時点では使用しないが予約

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private Button _retryButton;

    private void Awake()
    {
        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(() =>
            {
                // 現時点でシーンリロード等はせず、ログだけ出力する
                Debug.Log("Result Popup Closed / Retry");
                OnRetryClicked?.Invoke();
            });
        }
    }

    /// <summary>
    /// ポップアップを開き、リザルト内容を表示する
    /// </summary>
    public void Show(int score, string rank, int money, int exp)
    {
        gameObject.SetActive(true);

        if (_scoreText != null) _scoreText.text = $"Score: {score}";
        if (_rankText != null) _rankText.text = $"Rank: {rank}";
        if (_moneyText != null) _moneyText.text = $"Money: +{money}";
        if (_expText != null) _expText.text = $"Exp: +{exp}";
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

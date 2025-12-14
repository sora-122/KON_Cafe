using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _cafeButton;
    [SerializeField] private Button _rankButton;
    [SerializeField] private Button _managementButton;

    [Header("Popups")]
    [SerializeField] private GameObject _confirmPopup; // 遷移確認用
    [SerializeField] private GameObject _rankPopup;    // ランク確認用

    // ランクポップアップの中身
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private Button _popupCloseButton;
    [SerializeField] private Button _confirmYesButton;
    [SerializeField] private Button _confirmNoButton;

    // イベント通知
    public event Action OnCafeButtonClicked;
    public event Action OnRankButtonClicked;
    public event Action OnManagementButtonClicked;
    public event Action OnConfirmYesClicked;

    private void Start()
    {
        // メインボタン
        _cafeButton.onClick.AddListener(() => OnCafeButtonClicked?.Invoke());
        _rankButton.onClick.AddListener(() => OnRankButtonClicked?.Invoke());
        _managementButton.onClick.AddListener(() => OnManagementButtonClicked?.Invoke());

        // ポップアップ内ボタン
        _popupCloseButton.onClick.AddListener(HidePopups);
        _confirmNoButton.onClick.AddListener(HidePopups);
        _confirmYesButton.onClick.AddListener(() =>
        {
            HidePopups();
            OnConfirmYesClicked?.Invoke();
        });

        // 初期化: ポップアップを隠す
        HidePopups();
    }

    public void ShowConfirmPopup()
    {
        _confirmPopup.SetActive(true);
        _rankPopup.SetActive(false);
    }

    public void ShowRankPopup(int rank, int currentExp, int nextExp)
    {
        _rankText.text = $"Rank {rank}";
        _expText.text = $"EXP {currentExp} / {nextExp}";

        _rankPopup.SetActive(true);
        _confirmPopup.SetActive(false);
    }

    private void HidePopups()
    {
        _confirmPopup.SetActive(false);
        _rankPopup.SetActive(false);
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 個別の来店タスク (アニマルと注文) を表示するUI要素
/// </summary>
[RequireComponent(typeof(Button))] // Button 必須
public class TaskBarElement : MonoBehaviour
{
    // タスク ID を通知するクリックイベント
    public event Action<string> OnClicked;

    [SerializeField] private Image _animalIcon;
    [SerializeField] private TextMeshProUGUI _orderText;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _timerText;

    private string _taskId;
    private Button _button;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);

        // _backgroundImage が未設定なら自身の Image を取得
        if (_backgroundImage == null)
        {
            _backgroundImage = GetComponent<Image>();
        }
    }

    /// <summary>
    /// データを表示に反映する
    /// </summary>
    public void Initialize(CustomerTask task)
    {
        _taskId = task.TaskId;

        if (_animalIcon != null)
        {
            _animalIcon.sprite = task.Animal.Icon;
        }

        if (_orderText != null)
        {
            // 文字列連結は GC を生むが、初期化時の1回のみなので許容
            // 将来的に ZString 等のライブラリ導入を検討 (今回は標準で実装)
            _orderText.text = $"{task.OrderItem.DisplayName} x1";
        }

        // 生成時は「完了不可」として初期化
        SetCompletable(false);
    }

    /// <summary>
    /// 完了可能状態 (強調表示) を切り替える
    /// </summary>
    public void SetCompletable(bool isCompletable)
    {
        if (_button != null)
        {
            _button.interactable = isCompletable;
        }

        if (_backgroundImage != null)
        {
            // 注文の必要数に対してストック数が足りていれば見た目を変える (例: 緑色)
            _backgroundImage.color = isCompletable ? new Color(0.6f, 1f, 0.6f) : Color.white;
        }
    }

    /// <summary>
    /// 残り時間の表示を更新する
    /// </summary>
    public void UpdateTimer(float remainingSeconds)
    {
        if (_timerText != null)
        {
            // 整数で表示
            _timerText.text = Mathf.CeilToInt(remainingSeconds).ToString();

            // 残り5秒以下で見た目を変える (例: 赤色)
            _timerText.color = remainingSeconds <= 5.0f ? Color.red : Color.black;
        }
    }

    private void HandleClick()
    {
        OnClicked?.Invoke(_taskId);
    }
}

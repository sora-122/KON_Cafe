using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 個別のメニュー在庫数アイコンを表示する View クラス
/// アイコン画像と所持数テキストを管理する
/// </summary>
public class StockItemElement : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _countText;

    private int _currentCount = -1; // 初期値 -1 で初回更新を保証

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize(Sprite icon)
    {
        if (_iconImage != null)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = (icon != null);
        }

        UpdateCount(0); // 初期在庫は 0
    }

    /// <summary>
    /// 在庫数を表示更新する
    /// </summary>
    public void UpdateCount(int count)
    {
        // 変更がない場合は何もしない (GC/負荷対策)
        if (_currentCount == count) return;

        _currentCount = count;
        if (_countText != null)
        {
            _countText.text = _currentCount.ToString();
        }
    }
}

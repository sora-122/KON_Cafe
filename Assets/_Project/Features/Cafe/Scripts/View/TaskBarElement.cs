using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 個別の来店タスク (アニマルと注文) を表示するUI要素
/// </summary>
public class TaskBarElement : MonoBehaviour
{
    [SerializeField] private Image _animalIcon;
    [SerializeField] private TextMeshProUGUI _orderText;

    /// <summary>
    /// データを表示に反映する
    /// </summary>
    public void Initialize(AnimalData animal, MenuItemData orderItem)
    {
        if (_animalIcon != null)
        {
            _animalIcon.sprite = animal.Icon;
        }

        if (_orderText != null)
        {
            // 文字列連結は GC を生むが、初期化時の1回のみなので許容
            // 将来的に ZString 等のライブラリ導入を検討 (今回は標準で実装)
            _orderText.text = $"{orderItem.DisplayName} x1";
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// メニュー選択バー内に表示される、個々のメニュー項目 (アイコンボタン) を管理する View クラス
/// メニュー ID を保持し、クリック時に選択イベントを通知する
/// </summary>
public class MenuItemElement : MonoBehaviour
{
    // メニュー ID を通知するイベント
    public event Action<string> OnClicked;

    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _nameText;

    private string _menuId;

    private void Awake()
    {
        // 堅牢性: null チェック
        if (_button == null) _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    // 初期化メソッド (Instantiate 直後や再利用時に呼ぶ)
    public void Initialize(string menuId, string displayName)
    {
        _menuId = menuId;
        if (_nameText != null) _nameText.text = displayName;

        // アイコン設定などはここで行う
    }

    private void HandleClick()
    {
        // 文字列連結などの GC 発生処理は行わず、ID をそのまま通知する
        OnClicked?.Invoke(_menuId);
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メニュー作成スロットのUIコンポーネント
/// </summary>
[RequireComponent(typeof(Button))] // Button コンポ―ネントを必須にする
public class MenuSlot : MonoBehaviour
{
    // このスロットがクリックされたことを通知するイベント
    // (View が購読する)
    public event Action<MenuSlot> OnClicked; // 仕様: プレイヤーやアニマルのアイコン

    [SerializeField]
    private Image _iconImage;

    [SerializeField]
    private GameObject _cookingMask;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        // ボタンクリック時に OnClicked イベントを発火させる
        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        OnClicked?.Invoke(this);
    }

    /// <summary>
    /// スロット状態 (調理中かどうか) を切り替える
    /// </summary>
    public void SetCookingState(bool isCooking)
    {
        // 調理中はマスクを表示し、ボタンを押せないようにする
        if (_cookingMask != null) _cookingMask.SetActive(isCooking);
        _button.interactable = !isCooking;
    }
}
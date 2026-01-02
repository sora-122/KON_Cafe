using System;
using UnityEngine;
using UnityEngine.UI;

public class ManagementView : MonoBehaviour
{
    [SerializeField] private Button _backButton;

    public event Action OnBackButtonClicked;

    private void Start()
    {
        if (_backButton != null)
        {
            _backButton.onClick.AddListener(() => OnBackButtonClicked?.Invoke());
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 作成可能なメニューの一覧を表示する UI (メニュー選択バー) を表示する View クラス
/// オブジェクトプーリングを用いてメニュー項目の生成・表示を最適化し、GC 発生を抑制する
/// </summary>
public class MenuSelectionBar : MonoBehaviour
{
    public event Action<string> OnMenuSelected;

    [SerializeField] private MenuItemElement _elementPrefab;
    [SerializeField] private Transform _container;

    // GC 対策: 生成済みオブジェクトをプールするリスト
    private readonly List<MenuItemElement> _pooledElements = new List<MenuItemElement>();

    /// <summary>
    /// データを受け取ってリストを表示する
    /// </summary>
    public void Open(IReadOnlyList<MenuItemData> menuDataList)
    {
        gameObject.SetActive(true);

        // 1．必要な数だけ要素を用意する (不足分は生成、余剰分は非表示)
        // LINQ を使わず for ループで処理
        for (int i = 0; i < menuDataList.Count; i++)
        {
            MenuItemElement element;

            if (i < _pooledElements.Count)
            {
                // プールから再利用
                element = _pooledElements[i];
                element.gameObject.SetActive(true);
            }
            else
            {
                // 新規生成
                element = Instantiate(_elementPrefab, _container);

                // イベント購読 (破棄しないので一度だけで OK だが、重複登録に注意)
                // 簡易実装として、ここでは購読しっぱなしにする設計とする
                element.OnClicked += HandleElementClicked;

                _pooledElements.Add(element);
            }

            // データをセット
            var data = menuDataList[i];
            element.Initialize(data.ItemId, data.DisplayName);
        }

        // 余った要素を非表示にする
        for (int i = menuDataList.Count; i < _pooledElements.Count; i++)
        {
            _pooledElements[i].gameObject.SetActive(false);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void HandleElementClicked(string menuId)
    {
        OnMenuSelected?.Invoke(menuId);
    }
}

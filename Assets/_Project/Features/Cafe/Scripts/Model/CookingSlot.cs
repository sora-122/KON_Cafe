using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 1つの調理スロットの状態とロジックを管理する Model クラス
/// 非同期でタイマー処理を行う
/// </summary>
public class CookingSlot
{
    // 状態変化通知イベント
    public event Action<bool> OnStateChanged; // true: 調理開始, false: 調理完了
    public event Action<string> OnCookingCompleted; // 調理完了時のアイテム ID 通知

    public bool IsBusy { get; private set; }

    /// <summary>
    /// 調理を開始する
    /// </summary>
    public async UniTask StartCookingAsync(MenuItemData itemData, CancellationToken token)
    {
        // IsBusy が true (作成中) なら return する (実行しない)
        if (IsBusy) return;

        IsBusy = true;
        OnStateChanged?.Invoke(true); // View を「作成中」にする

        // 作成時間待機 (ミリ秒換算)
        int waitTimeMs = (int)(itemData.CreationTimeSeconds * 1000);

        try
        {
            // Update ループを使わず、UniTask で待機する (GCフリー)
            await UniTask.Delay(waitTimeMs, cancellationToken: token);

            // 完了処理
            IsBusy = false;
            OnStateChanged?.Invoke(false); // View を「空き」に戻す
            OnCookingCompleted?.Invoke(itemData.ItemId); // ストック加算通知
        }
        catch (OperationCanceledException)
        {
            // キャンセル時は状態をリセット
            IsBusy = false;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 発生したタスク一覧を表示・管理する View クラス
/// ObjectPool を使用し、GCAlloc を抑制する
/// </summary>
public class TaskListPanel : MonoBehaviour
{
    // どのタスクがクリックされたのかを View に通知
    public event Action<string> OnTaskClicked;

    [SerializeField] private TaskBarElement _elementPrefab;
    [SerializeField] private Transform _container;

    // ID検索用ディクショナリ (O(1)アクセス)
    // List から Dictionary に変更して検索を高速化
    private readonly Dictionary<string, TaskBarElement> _elements = new Dictionary<string, TaskBarElement>();

    // オブジェクトプールの定義
    private IObjectPool<TaskBarElement> _taskPool;

    private void Awake()
    {
        // プールの初期化
        _taskPool = new ObjectPool<TaskBarElement>(
            createFunc: () =>
            {
                var element = Instantiate(_elementPrefab, _container);
                return element;
            },
            actionOnGet: (element) =>
            {
                element.gameObject.SetActive(true);
                // 順序制御のため、Get時に最後の子要素として最下部に配置
                element.transform.SetAsLastSibling();
            },
            actionOnRelease: (element) =>
            {
                element.gameObject.SetActive(false);
            },
            actionOnDestroy: (element) =>
            {
                Destroy(element.gameObject);
            },
            collectionCheck: true, // 同一インスタンスの二重Releaseチェック
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    /// <summary>
    /// 新しいタスクを表示に追加する
    /// </summary>
    public void AddTask(CustomerTask task)
    {
        var element = _taskPool.Get();
        element.Initialize(task);

        // イベント購読
        element.OnClicked += HandleElementClicked;

        _elements.Add(task.TaskId, element);
    }

    /// <summary>
    /// 指定された ID のタスク表示を削除する
    /// <summary>
    public void RemoveTask(string taskId)
    {
        if (_elements.TryGetValue(taskId, out var element))
        {
            // イベント購読解除
            element.OnClicked -= HandleElementClicked;
            _elements.Remove(taskId);
            _taskPool.Release(element);
        }
    }

    /// <summary>
    /// 指定タスクの強調表示を更新する
    /// </summary>
    public void UpdateTaskState(string taskId, bool isCompletable)
    {
        if (_elements.TryGetValue(taskId, out var element))
        {
            element.SetCompletable(isCompletable);
        }
    }

    /// <summary>
    /// 指定タスクの時間表示を更新する
    /// </summary>
    public void UpdateTaskTime(string taskId, float remainingSeconds)
    {
        if (_elements.TryGetValue(taskId, out var element))
        {
            element.UpdateTimer(remainingSeconds);
        }
    }

    private void HandleElementClicked(string raskId)
    {
        OnTaskClicked?.Invoke(raskId);
    }
}

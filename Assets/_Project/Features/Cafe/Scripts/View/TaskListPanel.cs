using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 発生したタスク一覧を表示・管理する View クラス
/// </summary>
public class TaskListPanel : MonoBehaviour
{
    // どのタスクがクリックされたのかを View に通知
    public event Action<string> OnTaskClicked;

    [SerializeField] private TaskBarElement _elementPrefab;
    [SerializeField] private Transform _container;

    // ID検索用ディクショナリ (O(1)アクセス)
    // List から Dictionary に変更して検索を高速化
    // private readonly List<TaskBarElement> _activeElements = new List<TaskBarElement>();
    private readonly Dictionary<string, TaskBarElement> _elements = new Dictionary<string, TaskBarElement>();

    /// <summary>
    /// 新しいタスクを表示に追加する
    /// </summary>
    public void AddTask(CustomerTask task)
    {
        // 今回は単純に Instantiate (将来はプーリング化)
        var element = Instantiate(_elementPrefab, _container);
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
            // 破棄処理
            element.OnClicked -= HandleElementClicked;
            Destroy(element.gameObject);
            _elements.Remove(taskId);
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

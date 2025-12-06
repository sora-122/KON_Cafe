using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 発生したタスク一覧を表示・管理する View クラス
/// </summary>
public class TaskListPanel : MonoBehaviour
{
    [SerializeField] private TaskBarElement _elementPrefab;
    [SerializeField] private Transform _container;

    private readonly List<TaskBarElement> _activeElements = new List<TaskBarElement>();

    /// <summary>
    /// 新しいタスクを表示に追加する
    /// </summary>
    public void AddTask(CustomerTask task)
    {
        // 今回は単純に Instantiate (将来はプーリング化)
        var element = Instantiate(_elementPrefab, _container);
        element.Initialize(task.Animal, task.OrderItem);

        _activeElements.Add(element);
    }

    // TODO: 将来的に RemoveTask などを実装
}

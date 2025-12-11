/// <summary>
/// 来客した客アニマルと、その注文内容を保持する Model クラス
/// </summary>
public class CustomerTask
{
    public string TaskId { get; } // ユニークID (将来的に管理)
    public AnimalData Animal { get; }
    public MenuItemData OrderItem { get; }

    // 制限時間データ
    public float TotalTime { get; }
    public float RemainingTime { get; set; }


    public CustomerTask(string taskId, AnimalData animal, MenuItemData orderItem, float timeLimit)
    {
        TaskId = taskId;
        Animal = animal;
        OrderItem = orderItem;

        TotalTime = timeLimit;
        RemainingTime = timeLimit;
    }
}

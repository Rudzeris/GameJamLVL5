using UnityEngine;

public class MissionUI : MonoBehaviour
{
    private void OnEnable()
    {
        Messenger<string, string>.AddListener(GameEvent.TASK_PROGRESS, OnTaskProgress);
        Messenger<string>.AddListener(GameEvent.TASK_READY_FOR_DELIVERY, OnTaskReadyForDelivery);
        Messenger<string>.AddListener(GameEvent.TASK_COMPLETED, OnTaskCompleted);
    }

    private void OnDisable()
    {
        Messenger<string, string>.RemoveListener(GameEvent.TASK_PROGRESS, OnTaskProgress);
        Messenger<string>.RemoveListener(GameEvent.TASK_READY_FOR_DELIVERY, OnTaskReadyForDelivery);
        Messenger<string>.RemoveListener(GameEvent.TASK_COMPLETED, OnTaskCompleted);
    }

    private void OnTaskProgress(string taskTitle, string progress)
    {
        Debug.Log($"Progress updated for task '{taskTitle}': {progress}");
        // Здесь обновляй UI прогресс-бары, текст и т.п.
    }

    private void OnTaskReadyForDelivery(string taskTitle)
    {
        Debug.Log($"Task ready for delivery: {taskTitle}");
        // Здесь, например, включай кнопку сдачи задания
    }

    private void OnTaskCompleted(string taskTitle)
    {
        Debug.Log($"Task completed: {taskTitle}");
        // Обнови UI, убери задачу из списка или покажи "завершено"
    }
}

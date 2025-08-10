using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("Managers/MissionManager")]
public class MissionManager : MonoBehaviour, IGameManager
{
    // Статус менеджера по контракту IGameManager
    public ManagerStatus Status { get; private set; } = ManagerStatus.Shutdown;

    // Список активных задач
    public List<TaskData> Tasks = new();

    // Старт менеджера (вызывается из Managers.StartupManagers)
    public void Startup()
    {
        // Подписываемся на сбор предметов
        try
        {
            Messenger<ItemType, int>.AddListener(GameEvent.ITEM_COLLECTED, OnItemCollected);
        }
        catch (Exception ex)
        {
            Debug.LogError($"MissionManager subscribe error: {ex.Message}");
        }

        Status = ManagerStatus.Started;
        Debug.Log("[MissionManager] Started");
    }

    private void OnDestroy()
    {
        // Безопасно отписываемся
        try
        {
            Messenger<ItemType, int>.RemoveListener(GameEvent.ITEM_COLLECTED, OnItemCollected);
        }
        catch { /* игнорируем при выгрузке */ }
    }


    // Обработка события сбора предметов
    private void OnItemCollected(ItemType type, int amount)
    {
        foreach (var task in Tasks)
        {
            var prevStatus = task.Status;
            bool changed = task.OnItemCollected(type, amount);

            if (!changed && prevStatus == task.Status) continue;

            // Шлём прогресс задачи
            try
            {
                Messenger<string, string>.Broadcast(GameEvent.TASK_PROGRESS, task.Title, task.GetProgressString());
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"TASK_PROGRESS broadcast error: {ex.Message}");
            }

            // Если задача впервые стала готова к сдаче — сообщаем
            if (prevStatus != TaskStatus.ReadyForDelivery && task.Status == TaskStatus.ReadyForDelivery)
            {
                try
                {
                    Messenger<string>.Broadcast(GameEvent.TASK_READY_FOR_DELIVERY, task.Title);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"TASK_READY_FOR_DELIVERY broadcast error: {ex.Message}");
                }
            }
        }
    }

    // Попытка сдать задачи в точке доставки
    public bool TryDeliver(DeliveryPointType point)
    {
        bool deliveredAny = false;

        foreach (var task in Tasks)
        {
            if (task.TryDeliver(point))
            {
                deliveredAny = true;

                // Уведомляем о завершении
                try
                {
                    Messenger<string>.Broadcast(GameEvent.TASK_COMPLETED, task.Title);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"TASK_COMPLETED broadcast error: {ex.Message}");
                }

                Debug.Log($"[MissionManager] Task completed: {task.Title}");
            }
        }

        if (!deliveredAny)
            Debug.Log("[MissionManager] No tasks ready for delivery at this point");

        return deliveredAny;
    }

    public TaskStatus GetTaskStatus(DeliveryPointType point)
    {
        var task = Tasks.FirstOrDefault(t => t.DeliveryPoint == point);
        if (task == null)
        {
            Debug.LogWarning($"[MissionManager] No task found for delivery point {point}");
            return TaskStatus.NotStarted;
        }
        return task.Status;
    }
}
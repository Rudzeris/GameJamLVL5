using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("Managers/MissionManager")]
public class MissionManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; } = ManagerStatus.Shutdown;

    public List<TaskData> Tasks = new();

    public void Startup()
    {
        try
        {
            // Подписываемся на обновление инвентаря (сбор и расход)
            Messenger<ItemType, int>.AddListener(GameEvent.INVENTORY_UPDATED, OnInventoryChanged);
        }
        catch (Exception ex)
        {
            Debug.LogError($"MissionManager subscribe error: {ex.Message}");
        }

        Status = ManagerStatus.Started;
        Debug.Log("[MissionManager] Started");

        // Можно при старте сразу пересчитать прогресс по текущему инвентарю
        RecalculateAllProgress();
    }

    private void OnDestroy()
    {
        try
        {
            Messenger<ItemType, int>.RemoveListener(GameEvent.INVENTORY_UPDATED, OnInventoryChanged);
        }
        catch { }
    }

    private void OnInventoryChanged(ItemType type, int currentAmount)
    {
        bool anyStatusChanged = false;

        foreach (var task in Tasks)
        {
            var prevStatus = task.Status;

            // Обновляем прогресс задания в зависимости от реального количества предметов в инвентаре
            bool changed = task.UpdateProgress(type, currentAmount);

            if (changed)
                anyStatusChanged = true;

            if (changed || prevStatus != task.Status)
            {
                // Рассылаем прогресс
                Messenger<string, string>.Broadcast(GameEvent.TASK_PROGRESS, task.Title, task.GetProgressString());

                // Если задача стала готова к сдаче — уведомляем
                if (prevStatus != TaskStatus.ReadyForDelivery && task.Status == TaskStatus.ReadyForDelivery)
                {
                    Messenger<string>.Broadcast(GameEvent.TASK_READY_FOR_DELIVERY, task.Title);
                }
                // Если задача стала не готова (например, предметы потрачены), можно добавить обработку, если нужно
            }
        }
    }

    // При старте или необходимости можно пересчитать весь прогресс по инвентарю
    private void RecalculateAllProgress()
    {
        foreach (var task in Tasks)
        {
            foreach (var req in task.Requirements)
            {
                int currentAmount = Managers.Inventory.GetItemCount(req.Type);
                task.UpdateProgress(req.Type, currentAmount);
            }
        }
    }

    public bool TryDeliver(DeliveryPointType point)
    {
        bool deliveredAny = false;

        foreach (var task in Tasks)
        {
            if (task.DeliveryPoint == point && task.Status == TaskStatus.ReadyForDelivery)
            {
                // Проверяем, есть ли действительно нужные предметы в инвентаре
                bool hasAll = true;
                foreach (var req in task.Requirements)
                {
                    if (Managers.Inventory.GetItemCount(req.Type) < req.Amount)
                    {
                        hasAll = false;
                        break;
                    }
                }

                if (!hasAll)
                {
                    Debug.LogWarning($"[MissionManager] Not enough items to deliver task {task.Title}");
                    continue;
                }

                // Списываем предметы из инвентаря
                foreach (var req in task.Requirements)
                {
                    Managers.Inventory.RemoveItem(req.Type, req.Amount);
                }

                // Меняем статус задачи
                task.Status = TaskStatus.Completed;

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

                deliveredAny = true;
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

    public bool TryGetRequiredItems(DeliveryPointType point, out List<TaskRequirement> requiredItems)
    {
        requiredItems = null;
        var task = Tasks.FirstOrDefault(t => t.DeliveryPoint == point);

        if (task == null)
            return false;

        requiredItems = task.Requirements;
        return requiredItems != null && requiredItems.Count > 0;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Одно требование (тип предмета + количество)
[System.Serializable]
public class TaskRequirement
{
    public ItemType Type;
    public int Amount;

    public TaskRequirement(ItemType type, int amount)
    {
        Type = type;
        Amount = Mathf.Max(0, amount);
    }
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    ReadyForDelivery,
    Completed
}

// Описание одной задачи и её прогресс
[System.Serializable]
public class TaskData
{
    public string Title;
    public string Description;
    public DeliveryPointType DeliveryPoint;
    public List<TaskRequirement> Requirements = new();

    // Внутренний прогресс: ItemType -> собранное количество
    private readonly Dictionary<ItemType, int> progress = new();

    public TaskStatus Status { get;  set; } = TaskStatus.NotStarted;

    public TaskData()
    {
        progress = new();
    }

    public TaskData(string title, string description, DeliveryPointType point, IEnumerable<TaskRequirement> reqs) : this()  
    {
        Title = title;
        Description = description;
        DeliveryPoint = point;
        Requirements = reqs.ToList();
    }

    // Вызывается при сборе предмета
    public bool OnItemCollected(ItemType type, int amount)
    {
        if (Status == TaskStatus.Completed) return false;

        var req = Requirements.FirstOrDefault(r => r.Type == type);
        if (req == null) return false;

        if (!progress.ContainsKey(type)) progress[type] = 0;

        // Добавляем, но не превышаем требуемый максимум
        int before = progress[type];
        progress[type] = Mathf.Min(req.Amount, progress[type] + Mathf.Max(1, amount));
        bool changed = progress[type] != before;

        if (changed && Status == TaskStatus.NotStarted)
            Status = TaskStatus.InProgress;

        if (IsReadyForDelivery())
            Status = TaskStatus.ReadyForDelivery;

        return changed;
    }

    public bool IsReadyForDelivery()
    {
        foreach (var r in Requirements)
        {
            int current = progress.TryGetValue(r.Type, out var c) ? c : 0;
            if (current < r.Amount) return false;
        }
        return true;
    }

    // Попытка сдать: сверяем точку, статус и завершаем
    // В этой версии мы не трогаем инвентарь — считаем, что всё собрано для этой задачи
    // Если нужно списание из инвентаря — добавь вызов в свой InventoryManager здесь
    public bool TryDeliver(DeliveryPointType point)
    {
        if (Status != TaskStatus.ReadyForDelivery) return false;
        if (DeliveryPoint != point) return false;

        Status = TaskStatus.Completed;
        return true;
    }

    public string GetProgressString()
    {
        var parts = new List<string>();
        foreach (var r in Requirements)
        {
            var current = progress.TryGetValue(r.Type, out var c) ? c : 0;
            parts.Add($"{r.Type}: {current}/{r.Amount}");
        }
        return string.Join(", ", parts);
    }

    private void UpdateStatus()
    {
        if (Status == TaskStatus.Completed)
            return; // Уже выполнена

        bool anyProgress = false;
        bool allComplete = true;

        foreach (var r in Requirements)
        {
            int current = progress.TryGetValue(r.Type, out var c) ? c : 0;

            if (current > 0)
                anyProgress = true;

            if (current < r.Amount)
                allComplete = false;
        }

        if (allComplete)
            Status = TaskStatus.ReadyForDelivery;
        else if (anyProgress)
            Status = TaskStatus.InProgress;
        else
            Status = TaskStatus.NotStarted;
    }

    public bool UpdateProgress(ItemType type, int currentAmount)
    {
        var req = Requirements.FirstOrDefault(r => r.Type == type);
        if (req == null)
            return false;

        int oldProgress = progress.TryGetValue(type, out var p) ? p : 0;
        int newProgress = Mathf.Min(currentAmount, req.Amount);
        progress[type] = newProgress;

        bool changed = oldProgress != newProgress;

        if (changed)
            UpdateStatus();

        return changed;
    }
}
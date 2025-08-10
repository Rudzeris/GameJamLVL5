using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Managers/Inventory")]
public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    private Dictionary<ItemType, int> items = new();

    public void Startup()
    {
        Debug.Log("Inventory Manager starting...");
        Status = ManagerStatus.Started;

        Messenger<ItemType, int>.AddListener(GameEvent.ITEM_COLLECTED, AddItem);
    }

    public void Shutdown()
    {
        Messenger<ItemType, int>.RemoveListener(GameEvent.ITEM_COLLECTED, AddItem);
    }

    // Добавление предмета
    public void AddItem(ItemType type, int amount)
    {

        if (!items.ContainsKey(type))
            items[type] = 0;

        items[type] += amount;

        Messenger<ItemType, int>.Broadcast(GameEvent.INVENTORY_UPDATED, type, items[type]);
    }

    // Удаление предмета
    public bool RemoveItem(ItemType type, int amount)
    {
        if (!items.ContainsKey(type) || items[type] < amount)
        {
            Debug.LogWarning($"Not enough {type} to remove!");
            return false; // не удалось удалить
        }

        items[type] -= amount;
        if (items[type] <= 0)
            items.Remove(type);

        Messenger<ItemType, int>.Broadcast(GameEvent.INVENTORY_UPDATED, type, items.ContainsKey(type) ? items[type] : 0);

        return true; // успешно удалили
    }

    // Проверка, есть ли предмет
    public bool HasItem(ItemType type, int amount = 1)
    {
        return items.ContainsKey(type) && items[type] >= amount;
    }

    // Получить список всех предметов
    public Dictionary<ItemType, int> GetAllItems()
    {
        return new Dictionary<ItemType, int>(items);
    }

    public int GetItemCount(ItemType type)
    {
        if (items.TryGetValue(type, out int count))
            return count;
        return 0;
    }
}

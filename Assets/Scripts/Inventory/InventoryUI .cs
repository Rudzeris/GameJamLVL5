using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsParent;
    public ItemDatabase itemDatabase;

    private Dictionary<ItemType, InventorySlot> slotsMap = new();

    private void OnEnable()
    {
        Messenger<ItemType, int>.AddListener(GameEvent.INVENTORY_UPDATED, OnInventoryChanged);
    }

    private void OnDisable()
    {
        Messenger<ItemType, int>.RemoveListener(GameEvent.INVENTORY_UPDATED, OnInventoryChanged);
    }

    private void OnInventoryChanged(ItemType type, int amount)
    {
        Debug.Log("Changed");
        if (amount > 0)
            AddOrUpdateSlot(type, amount);
        else
            RemoveSlot(type);
    }

    private void AddOrUpdateSlot(ItemType type, int amount)
    {
        if (slotsMap.TryGetValue(type, out var slot))
        {
            slot.Setup(type, amount, SlotMode.Inventory);
        }
        else
        {
            var slotObj = Instantiate(slotPrefab, slotsParent);
            var newSlot = slotObj.GetComponent<InventorySlot>();
            newSlot.itemDatabase = itemDatabase; // чтобы подцепить иконки
            newSlot.Setup(type, amount, SlotMode.Inventory);
            slotsMap[type] = newSlot;
        }
    }

    private void RemoveSlot(ItemType type)
    {
        if (slotsMap.TryGetValue(type, out var slot))
        {
            Destroy(slot.gameObject);
            slotsMap.Remove(type);
        }
    }
}

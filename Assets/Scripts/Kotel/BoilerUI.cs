using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoilerUI : MonoBehaviour, IDropHandler
{
    public Boiler boiler;
    private InventoryManager inventoryManager;

    public Transform slotsParent;
    public GameObject slotPrefab;
    public ItemDatabase itemDatabase;

    private Dictionary<ItemType, InventorySlot> slotsMap = new();

    public void Start()
    {
        inventoryManager = Managers.Inventory;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        if (draggedSlot == null) return;

        var itemType = draggedSlot.ItemType;
        int amountToAdd = 1; // Можно настроить

        if (inventoryManager.HasItem(itemType, amountToAdd))
        {
            if (boiler.AddIngredient(itemType, amountToAdd))
            {
                inventoryManager.RemoveItem(itemType, amountToAdd);
                UpdateBoilerUI(itemType);
            }
        }
    }

    private void UpdateBoilerUI(ItemType type)
    {
        if (slotsMap.TryGetValue(type, out var slot))
        {
            slot.Setup(type, boiler.GetIngredientAmount(type), SlotMode.Boiler);
        }
        else
        {
            var obj = Instantiate(slotPrefab, slotsParent);
            slot = obj.GetComponent<InventorySlot>();
            slot.Setup(type, boiler.GetIngredientAmount(type), SlotMode.Boiler);
            slotsMap[type] = slot;
        }
    }

    // Новый метод — вернуть ингредиенты в инвентарь
    public void ReturnIngredientsToInventory()
    {
        foreach (var kvp in boiler.GetAllIngredients())
        {
            inventoryManager.AddItem(kvp.Key, kvp.Value);
        }
        boiler.ClearIngredients();

        ClearBoilerUI();
    }

    // Очистить UI котла
    public void ClearBoilerUI()
    {
        foreach (var slot in slotsMap.Values)
        {
            Destroy(slot.gameObject);
        }
        slotsMap.Clear();
    }

    // При закрытии UI — возвращаем ингредиенты
    private void OnDisable()
    {
        ReturnIngredientsToInventory();
    }


}

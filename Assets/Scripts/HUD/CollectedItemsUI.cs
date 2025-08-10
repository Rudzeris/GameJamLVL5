using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectedItemsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectedItemsText;

    // Хранит текущие собранные предметы
    private Dictionary<ItemType, int> collectedItems = new();

    private void OnEnable()
    {
        Messenger<ItemType, int>.AddListener(GameEvent.ITEM_COLLECTED, OnItemCollected);
    }

    private void OnDisable()
    {
        Messenger<ItemType, int>.RemoveListener(GameEvent.ITEM_COLLECTED, OnItemCollected);
    }

    private void OnItemCollected(ItemType type, int amount)
    {
        if (!collectedItems.ContainsKey(type))
            collectedItems[type] = 0;

        collectedItems[type] += amount;

        UpdateUI();
    }

    private void UpdateUI()
    {
        var lines = new List<string>();
        foreach (var kvp in collectedItems)
        {
            lines.Add($"{kvp.Key}: {kvp.Value}");
        }

        collectedItemsText.text = string.Join("\n", lines);
    }
}

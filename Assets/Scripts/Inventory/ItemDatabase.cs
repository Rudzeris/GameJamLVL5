using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemEntry
    {
        public ItemType Type;
        public Sprite Icon;
        public string DisplayName;
        // Можно добавить и другие данные: описание, цена и т.п.
    }

    [SerializeField]
    private List<ItemEntry> items = new List<ItemEntry>();

    private Dictionary<ItemType, ItemEntry> lookup;

    private void OnEnable()
    {
        lookup = new Dictionary<ItemType, ItemEntry>();
        foreach (var item in items)
        {
            if (!lookup.ContainsKey(item.Type))
                lookup.Add(item.Type, item);
        }
    }

    public Sprite GetIcon(ItemType type)
    {
        if (lookup.TryGetValue(type, out var entry))
            return entry.Icon;
        else
            return null; // или дефолтная иконка
    }

    public string GetDisplayName(ItemType type)
    {
        if (lookup.TryGetValue(type, out var entry))
            return entry.DisplayName;
        else
            return type.ToString();
    }
}

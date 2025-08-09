using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    public ItemType Type = ItemType.Grain;
    public int Amount = 1;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        try
        {
            Messenger<ItemType, int>.Broadcast(GameEvent.ITEM_COLLECTED, Type, Amount);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"CollectibleItem Broadcast error: {ex.Message}");
        }


        Destroy(gameObject);
    }
}
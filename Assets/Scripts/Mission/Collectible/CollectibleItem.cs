using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour, IInteractable
{
    public ItemType Type = ItemType.Grain;
    public int Amount = 1;

    private bool isCollected = false; // Чтобы избежать повторного сбора

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void Activate()
    {
        Debug.Log($"Activate called on object: {gameObject.name} (ID: {gameObject.GetInstanceID()})");

        Messenger<ItemType, int>.Broadcast(GameEvent.ITEM_COLLECTED, Type, Amount);

        Destroy(gameObject);
        Debug.Log($"Destroy called on object: {gameObject.name} (ID: {gameObject.GetInstanceID()})");
    }
}

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeliveryZone : MonoBehaviour
{
    public DeliveryPointType PointType = DeliveryPointType.Chicken;

    private void Reset()
    {
        // Делаем триггером для детекции входа игрока
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Managers.Game == null) return;

        Managers.Mission.TryDeliver(PointType);
    }
}
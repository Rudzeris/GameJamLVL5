using cherrydev;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeliveryZone : MonoBehaviour, IInteractable
{
    public DeliveryPointType PointType = DeliveryPointType.Chicken;
    public DialogNodeGraph Progress;
    public DialogNodeGraph Ready;
    public DialogNodeGraph Complete;

    private void Reset()
    {
        // Делаем триггером для детекции входа игрока
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
    }

    public void Activate()
    {
        if (Managers.Game == null) return;

        var status = Managers.Mission.GetTaskStatus(PointType);
        Managers.Mission.TryDeliver(PointType);
        Debug.Log(status);
        switch (status)
        {
            case TaskStatus.NotStarted:
                Managers.Dialog.StartDialog(Progress);
                
                break;
            case TaskStatus.InProgress:
                Managers.Dialog.StartDialog(Progress);
                break;
            case TaskStatus.ReadyForDelivery: 
                Managers.Dialog.StartDialog(Ready);
                break;
            case TaskStatus.Completed:
                Managers.Dialog.StartDialog(Complete);
                break;
        }
    }
}
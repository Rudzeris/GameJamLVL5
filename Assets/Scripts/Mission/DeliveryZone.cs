using cherrydev;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemReward
{
    public ItemType itemType;
    public int amount;
}

[RequireComponent(typeof(Collider2D))]
public class DeliveryZone : MonoBehaviour, IInteractable
{
    public DeliveryPointType PointType = DeliveryPointType.Chicken;
    public List<ItemReward> reward;
    public DialogNodeGraph Progress;
    public DialogNodeGraph Ready;
    public DialogNodeGraph Complete;

    private void Reset()
    {
        // Делаем триггером для детекции входа игрока
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
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
                GiveReward();
                break;
            case TaskStatus.Completed:
                Managers.Dialog.StartDialog(Complete);
                break;
        }
    }

    private void GiveReward()
    {
        foreach(var child in reward)
        {
            Messenger<ItemType, int>.Broadcast(GameEvent.ITEM_COLLECTED, child.itemType, child.amount);
        }
    }
}
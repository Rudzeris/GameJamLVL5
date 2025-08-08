using UnityEngine;

[AddComponentMenu("Managers/Inventory")]
public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    public void Startup()
    {
        Debug.Log("Inventory Manager starting...");
        Status = ManagerStatus.Started;
    }
}
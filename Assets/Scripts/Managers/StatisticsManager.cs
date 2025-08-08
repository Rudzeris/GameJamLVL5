using UnityEngine;

[AddComponentMenu("Managers/Statistics")]
public class StatisticsManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }
    public void Startup()
    {
        Debug.Log("Statistics Manager starting...");
        Status = ManagerStatus.Started;
    }
}
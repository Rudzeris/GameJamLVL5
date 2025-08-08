using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    private string _filename;

    public void Startup()
    {
        Debug.Log("DataManager starting...");

        _filename = Path.Combine(Application.persistentDataPath, "game_data.dat");
        Status = ManagerStatus.Started;
    }

    public void SaveGameState()
    {
        Debug.LogError("Saving is not implemented");
    }
    public void LoadGameState()
    {
        Debug.LogError("Loading is not implemented");
    }
}

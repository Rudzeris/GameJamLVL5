using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    public int curLevel { get; private set; }
    public int maxLevel { get; private set; }

    public void Startup()
    {
        Debug.Log("MissionManager starting...");

        UpdateData(0, 2);

        Status = ManagerStatus.Started;
    }

    public void UpdateData(int curLevel, int maxLevel)
    {
        this.curLevel = curLevel;
        this.maxLevel = maxLevel;
    }

    public void RestartCurrent()
    {
        string name = "Level " + curLevel;
        Debug.Log($"Restarting current level: {name}");
        SceneManager.LoadScene(name);
    }
    public void OpenLevel()
    {
        if (curLevel < maxLevel)
        {
            curLevel++;
            string name = "Level " + curLevel;
            Debug.Log($"Going to next level: {name}");
            SceneManager.LoadScene(name);
        }
        else
        {
            Debug.LogWarning("No more levels to go to.");
            try
            {
                Messenger.Broadcast(GameEvent.GAME_COMPLETE);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error: {ex.Message}");
            }
        }
    }
    public void MainMenu()
    {
        Debug.Log($"Going to Main Menu");
        SceneManager.LoadScene("MainMenu");
    }
    public void ExitGame()
    {
        Debug.Log("The game is coming to an end");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

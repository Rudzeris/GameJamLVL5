using cherrydev;
using UnityEditor;
using UnityEngine;

public class DialogManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; } = ManagerStatus.Shutdown;
    private DialogBehaviour dialogBehaviour;
    public DialogBehaviour dialogPrefab;

    public void Startup()
    {
        // Тут инициализация диалогов
        Status = ManagerStatus.Started;
        
    }

    public void ResetDialog()
    {
        dialogBehaviour = GameObject.Instantiate(dialogPrefab);
    }

    public void StartDialog(DialogNodeGraph dialog)
    {
        dialogBehaviour.StartDialog(dialog);
    }
}
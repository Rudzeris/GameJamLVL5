using cherrydev;
using UnityEditor;
using UnityEngine;

public class DialogManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; } = ManagerStatus.Shutdown;
    public DialogBehaviour dialogBehaviour;

    public void Startup()
    {
        // Тут инициализация диалогов
        Status = ManagerStatus.Started;
    }

    public void StartDialog(DialogNodeGraph dialog)
    {
        dialogBehaviour.StartDialog(dialog);
    }
}
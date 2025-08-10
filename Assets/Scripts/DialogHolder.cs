using cherrydev;
using UnityEngine;

public class DialogHolder : MonoBehaviour, IInteractable
{
    public DialogNodeGraph dialogGraph;

    public void Activate()
    {
        Managers.Dialog.dialogBehaviour.StartDialog(dialogGraph);
    }
}

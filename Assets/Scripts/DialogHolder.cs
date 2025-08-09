using cherrydev;
using UnityEngine;

public class DialogHolder : MonoBehaviour, IInteractable
{
    public DialogNodeGraph dialogGraph;
    public DialogBehaviour dialogBehaviour;

    public void Activate()
    {
        dialogBehaviour.StartDialog(dialogGraph);
    }
}

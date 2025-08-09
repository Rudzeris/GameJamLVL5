using cherrydev;
using UnityEngine;

public class DialogHolder : MonoBehaviour, ITouchable
{
    public DialogNodeGraph dialogGraph;
    public DialogBehaviour dialogBehaviour;

    public void Activate()
    {
        dialogBehaviour.StartDialog(dialogGraph);
    }
}

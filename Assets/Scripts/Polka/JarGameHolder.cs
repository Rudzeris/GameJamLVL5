using cherrydev;
using System;
using UnityEngine;

public class JarGameHolder : MonoBehaviour, IInteractable
{
    public GameObject jarGame;
    private bool isActivate = false;
    private bool isComplete = false;
    public DialogNodeGraph Start;
    public DialogNodeGraph End;

    public void OnEnable()
    {
        Messenger<bool>.AddListener(GameEvent.JAR_GAME_COMPLETE, OnGameComplete);
    }

    private void OnGameComplete(bool success)
    {
        if (success)
        {
            Managers.Dialog.StartDialog(End);
            isComplete = true;
        }
        else
        {
            Managers.Game.RestartCurrent();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        jarGame.SetActive(false);
        isActivate = false;
    }

    public void Activate()
    {
        if(!isComplete)
        {
            Managers.Dialog.StartDialog(Start);
        }
        else
        {
            Managers.Dialog.StartDialog(End);
        }

        isActivate = !isActivate;
        jarGame.SetActive(isActivate);
    }

}

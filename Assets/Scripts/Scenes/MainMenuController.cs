using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Sound when the button is clicked")]
    [SerializeField] private AudioClip buttonClickClip;
    public void StartGame()
    {
        Managers.Audio.PlaySound(buttonClickClip);
        Managers.Game.OpenLevel();
    }
    public void ExitGame()
    {
        Managers.Audio.PlaySound(buttonClickClip);
        Managers.Game.ExitGame();
    }
}

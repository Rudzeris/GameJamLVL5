using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Sound when the button is clicked")]
    [SerializeField] private AudioClip buttonClickClip;
    public void MainMenu()
    {
        Managers.Audio.PlaySound(buttonClickClip);
        Managers.Game.MainMenu();
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMover : MonoBehaviour
{
    public Vector2 newCameraPos = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            { 
                    var camera = new Vector3(newCameraPos.x, newCameraPos.y, -10);
                    Camera.main.transform.DOMove(camera, 0.1f).SetEase(Ease.InOutSine);
            }
        }
    }
}

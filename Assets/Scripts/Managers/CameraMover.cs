using DG.Tweening;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Vector3 posOffset = Vector3.zero;
    private bool isBack = false;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                var camera = Camera.main.gameObject.transform.position + posOffset * (isBack ? -1 : 1);
                Camera.main.transform.DOMove(camera, 0.1f).SetEase(Ease.InOutSine);
                isBack = !isBack;
            }
        }
    }
}

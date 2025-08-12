using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    private GameObject highlightedObject;
    private Renderer highlightedRenderer;
    private Tween highlightTween;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHighlight(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == highlightedObject)
        {
            ClearHighlight();
        }
    }

    private void TryHighlight(GameObject obj)
    {
        if(obj.GetComponent<IInteractable>() == null)
        {
            ClearHighlight();
            return;
        }
            

        if (highlightedObject == obj)
            return; // Уже подсвечен этот объект

        ClearHighlight();

        highlightedObject = obj;
        highlightedRenderer = obj.GetComponent<Renderer>();

        if (highlightedRenderer != null)
        {
            highlightTween = highlightedRenderer.material.DOColor(Color.yellow, "_Color", 0.5f)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void ClearHighlight()
    {
        if (highlightTween != null)
        {
            highlightTween.Kill();
            highlightTween = null;
        }

        if (highlightedRenderer != null)
        {
            highlightedRenderer.material.DOColor(Color.white, "_Color", 0.2f);
            highlightedRenderer = null;
        }

        highlightedObject = null;
    }

    public void Interact()
    {
        if (highlightedObject != null)
        {
            var interactable = highlightedObject.GetComponents<IInteractable>();
            foreach (var item in interactable)
            {
                if (item != null)
                {
                    item.Activate();
                }
            }
            
        }
    }
}

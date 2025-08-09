using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    private Collider2D InteractTriggerCollider;
    private GameObject insideTrigger;

    private void Awake()
    {
        InteractTriggerCollider = GetComponent<Collider2D> ();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        insideTrigger = null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        insideTrigger = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        insideTrigger = null;
    }

    public void Interact()
    {

        if (insideTrigger != null)
        {
            if (insideTrigger.GetComponent<ITouchable>() != null)
            {
                insideTrigger.GetComponent<ITouchable>().Activate();
            }
        }
    }

}

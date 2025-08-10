using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragPlayer2D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float followSpeed = 20f;
    [SerializeField] private bool useRigidbodyIfPresent = true;

    private Rigidbody2D rb;
    private bool dragging;
    private Vector3 dragOffset;
    private Vector2 targetPos;
    private float originalGravity;
    private bool originalKinematic;
    private RigidbodyConstraints2D originalConstraints;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        if (cam == null) return;
        Vector3 cursorWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
        dragOffset = transform.position - cursorWorld;
        dragging = true;

        if (useRigidbodyIfPresent && rb != null)
        {
            originalGravity = rb.gravityScale;
            originalKinematic = rb.isKinematic;
            originalConstraints = rb.constraints;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void OnMouseDrag()
    {
        if (!dragging || cam == null) return;
        Vector3 cursorWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
        Vector3 desired = cursorWorld + dragOffset;
        targetPos = desired;
        if (rb == null || !useRigidbodyIfPresent)
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (!dragging || rb == null || !useRigidbodyIfPresent) return;
        float t = 1f - Mathf.Exp(-followSpeed * Time.fixedDeltaTime);
        Vector2 newPos = Vector2.Lerp(rb.position, targetPos, t);
        rb.MovePosition(newPos);
    }

    void OnMouseUp()
    {
        if (!dragging) return;
        dragging = false;

        if (useRigidbodyIfPresent && rb != null)
        {
            rb.isKinematic = originalKinematic;
            rb.gravityScale = originalGravity;
            rb.constraints = originalConstraints;
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement 
{
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator;
    private float moveSpeed = 1;

    public PlayerMovement(Rigidbody2D rb, Animator animator)
    {
        this.rb = rb;
        this.animator = animator;
    }

    public void Move()
    {
        rb.linearVelocity = new Vector2(movementInput.x , movementInput.y) * moveSpeed;
    }

    public void Update(Vector2 value, float speed)
    {
        movementInput = value;
        moveSpeed = speed;

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);

        if (movementInput != Vector2.zero)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts
{
    public class InputManager : MonoBehaviour
    {
        private PlayerMovement playerMovement;
        private Rigidbody2D rb;
        private Animator animator;
        private Vector2 lookDirection;
        public PlayerInteraction playerInteraction;
        public float moveSpeed = 1;

        public void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            playerMovement = new PlayerMovement(rb, animator);
        }

        public void Update()
        {

       
        }

        private void FixedUpdate()
        {
            playerMovement.Move();
        }

        private void OnMove(InputValue value)
        {
            playerMovement.Update(value.Get<Vector2>(), moveSpeed);
        }

        private void OnInteract()
        {
            playerInteraction.Interact();
        }

        private void OnLook(InputValue Value)
        {
            Vector2 playerPosition = transform.position; 
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Value.Get<Vector2>());

            lookDirection = (worldPosition - playerPosition).normalized;
        }

       private void StopAttack()
        {
            animator.SetBool("Attack", false);
        }
    }
}

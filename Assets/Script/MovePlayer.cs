using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 movement;
    private Vector2 lastMovement;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
       
        
            // Lấy input di chuyển
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetBool("IsMoving", movement != Vector2.zero);

            if (movement != Vector2.zero)
            {
                lastMovement = movement;
                animator.SetFloat("X", lastMovement.x);
                animator.SetFloat("Y", lastMovement.y);
            }

            if (Input.GetMouseButtonDown(0))
            {  
                animator.SetTrigger("IsAttacking");
                movement = Vector2.zero;
                animator.SetBool("IsMoving", false);
            }
        
    }

    void FixedUpdate()
    {
       
        
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        
    }

  
}

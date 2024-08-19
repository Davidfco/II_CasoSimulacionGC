using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    Vector2 movement;

    void Start()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Set isMoving for Animator
        bool isMoving = movement.sqrMagnitude > 0;
        animator.SetBool("isMoving", isMoving);

        // Update Animator parameters for direction
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);

        // Flip the sprite based on horizontal movement (X-axis)
        if (movement.x != 0)
        {
            spriteRenderer.flipX = movement.x < 0;
        }

        // Flip the sprite vertically based on vertical movement (Y-axis)
        if (movement.y > 0)
        {
            // Moving upwards, flip the sprite on the Y-axis
            spriteRenderer.flipY = true;
        }
        else if (movement.y < 0)
        {
            // Moving downwards, reset the flip on the Y-axis
            spriteRenderer.flipY = false;
        }
    }

    void FixedUpdate()
    {
        
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}

using System.Collections;
using UnityEngine;

public class EnemyController2D : MonoBehaviour
{
    // Enemy's health
    [SerializeField]
    private float health;

    // Damage dealt by melee attacks
    [SerializeField]
    private float meleeDamage;

    // Distance at which the enemy becomes aggressive towards the player
    [SerializeField]
    private float aggroRange;

    // Distance at which the enemy will return to its original position if the player is out of range
    [SerializeField]
    private float returnRange;

    // Determines if the enemy is initially facing right
    [SerializeField]
    private bool isFacingRight = true;

    // Reference to the player character
    [SerializeField]
    private Transform player;

    // Size of the area used to check if the enemy is on the ground
    [SerializeField]
    private Vector2 groundCheckSize;

    // Position of the ground check area
    [SerializeField]
    private Transform groundCheck;

    // LayerMask to define which layers are considered ground
    [SerializeField]
    private LayerMask groundLayer;

    // LayerMask to define which layers should be ignored by raycasts
    [SerializeField]
    private LayerMask ignoreLayer;

    // Speed at which the enemy moves
    [SerializeField]
    private float movementSpeed;

    // Distance used to detect edges (to prevent falling off)
    [SerializeField]
    private float edgeDetectionDistance;

    // Range within which the enemy can perform melee attacks
    [SerializeField]
    private float meleeAttackRange;

    // Reference to the Animator component for animations
    [SerializeField]
    private Animator animator;

    private bool isAggro = false; // Indicates if the enemy is currently aggressive
    private Vector3 originalPosition; // Stores the enemy's starting position
    private Rigidbody2D rb;
    private bool isAttacking = false; // Prevents multiple attacks happening at once

    // Checks if the enemy is on the ground
    private bool IsGrounded()
    {
        // Uses OverlapBox to check for ground below the enemy
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0.0f, groundLayer);
    }

    // Checks if the player is grounded
    private bool IsPlayerGrounded()
    {
        // Gets the CharacterController2D component from the player
        CharacterController2D playerController = player.GetComponent<CharacterController2D>();
        if (playerController != null)
        {
            return playerController.IsGrounded();
        }
        return true; // Defaults to true if the player script is not found
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to this GameObject
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevents the enemy from rotating
        originalPosition = transform.position; // Save the initial position of the enemy
        animator = GetComponentInChildren<Animator>(); // Get the Animator component from the child GameObject
    }

    void Update()
    {
        if (!IsGrounded())
        {
            // Stops horizontal movement if the enemy is not on the ground
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= aggroRange)
        {
            isAggro = true; // Enemy becomes aggressive if within aggro range
        }
        else if (distanceToPlayer > returnRange)
        {
            isAggro = false;
            ReturnToOriginalPosition(); // Return to original position if out of range
        }

        if (isAggro && IsGrounded())
        {
            if (distanceToPlayer <= meleeAttackRange)
            {
                // Perform melee attack if in range and not already attacking
                if (!isAttacking)
                {
                    StartCoroutine(MeleeAttack());
                }
            }
            else
            {
                // Chase the player if in aggro range but not within attack range
                ChasePlayer();
            }
        }
        else
        {
            // Return to original position if not aggressive
            if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
            {
                ReturnToOriginalPosition();
            }
            else
            {
                animator.SetBool("isWalking", false); // Stop walking animation when at original position
            }
        }

        bool isNearEdge = CheckIfNearEdge();

        // Stop horizontal movement if near an edge
        if (isNearEdge)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // Handles flipping the enemy sprite based on movement direction
    private void HandleRotate()
    {
        float moveInput = rb.velocity.x;

        if (moveInput > 0 && !isFacingRight)
        {
            Flip(); // Flip the sprite to face right
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip(); // Flip the sprite to face left
        }
    }

    // Flips the enemy sprite
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Inverts the x scale to flip the sprite
        transform.localScale = localScale;
    }

    // Moves the enemy back to its original position
    private void ReturnToOriginalPosition()
    {
        Vector3 direction = (originalPosition - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        HandleRotate(); // Ensure the enemy is facing the right direction
        animator.SetBool("isWalking", true); // Start walking animation

        // Stop movement if the enemy is close to the original position
        if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false); // Stop walking animation
        }
    }

    // Makes the enemy chase the player
    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * movementSpeed;
        HandleRotate(); // Ensure the enemy is facing the right direction
        animator.SetBool("isWalking", true); // Start walking animation
    }

    // Checks if the enemy is near an edge
    private bool CheckIfNearEdge()
    {
        int edgeDetectionMask = ~ignoreLayer; // Invert ignoreLayer to ignore these layers in edge detection

        // Cast a ray downward to check if there's ground directly below
        if (!Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, edgeDetectionMask))
        {
            return true; // No ground below, assume edge
        }

        // Cast rays forward to check for ground in front of the enemy
        Vector2 forwardDirection = transform.right; // Assuming right is forward for 2D
        if (!Physics2D.Raycast(transform.position, forwardDirection, edgeDetectionDistance, edgeDetectionMask))
        {
            return true; // No ground detected in front, assume edge
        }

        return false; // Ground detected, not near an edge
    }

    // Performs a melee attack
    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking"); // Trigger attack animation

        // Wait for the attack animation to complete
        yield return new WaitForSeconds(1.0f); // Adjust based on your attack animation duration

        // Apply melee damage if the player is still within attack range
        if (Vector3.Distance(transform.position, player.position) <= meleeAttackRange)
        {
            CharacterController2D playerController = player.GetComponent<CharacterController2D>();
            if (playerController != null)
            {
                playerController.TakeDamage(meleeDamage); // Apply damage to the player
            }
        }

        isAttacking = false;
    }

    // Applies damage to the enemy
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // Handles enemy death
    public void Die()
    {
<<<<<<< Updated upstream
        animator.SetTrigger("isDead"); 
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 1f); 
=======
        animator.SetTrigger("isDead"); // Trigger death animation
        rb.velocity = Vector2.zero; // Stop movement
        Destroy(gameObject, 1f); // Destroy the enemy object after a delay to allow the death animation to play
>>>>>>> Stashed changes
    }
}

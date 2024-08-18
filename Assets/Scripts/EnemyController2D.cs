using System.Collections;
using UnityEngine;

public class EnemyController2D : MonoBehaviour
{
    [SerializeField]
    private float health;

    [SerializeField]
    private float meleeDamage;

    [SerializeField]
    private float aggroRange;

    [SerializeField]
    private float returnRange;

    [SerializeField]
    private bool isFacingRight = true; // Initial facing direction

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Vector2 groundCheckSize; // Size of the ground check area

    [SerializeField]
    private Transform groundCheck; // Transform representing the center of the ground check area

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float movementSpeed; // Movement speed

    [SerializeField]
    private float edgeDetectionDistance; // Distance to check for edges

    [SerializeField]
    private float meleeAttackRange; // Range within which the enemy can attack

    [SerializeField]
    private Animator animator; // Reference to the Animator component

    private bool isAggro = false;
    private Vector3 originalPosition; // Store the original position
    private Rigidbody2D rb;
    private bool isAttacking = false; // To prevent multiple attacks

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Prevent falling due to gravity
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        originalPosition = transform.position;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= aggroRange)
        {
            isAggro = true;
        }
        else if (distanceToPlayer > returnRange)
        {
            isAggro = false;
            ReturnToOriginalPosition();
        }

        if (isAggro)
        {
            if (distanceToPlayer <= meleeAttackRange)
            {
                // If in melee range, perform attack
                if (!isAttacking)
                {
                    StartCoroutine(MeleeAttack());
                }
            }
            else
            {
                // Continue chasing the player if in range but not attacking
                ChasePlayer();
            }
        }
        else
        {
            // Check if the zombie is moving towards the original position
            if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
            {
                // Move towards the original position and ensure the walking animation plays
                ReturnToOriginalPosition();
            }
            else
            {
                animator.SetBool("isWalking", false); // Stop walking animation when at the original position
            }
        }

        bool isGrounded = CheckIfGrounded();
        bool isNearEdge = CheckIfNearEdge();

        // Prevent falling off the edge by stopping movement if near an edge
        if (isNearEdge)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement if near an edge
        }
    }

    private void HandleRotate()
    {
        // Determine direction based on velocity
        float moveInput = rb.velocity.x;

        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Flip the sprite
        transform.localScale = localScale;
    }

    private void ReturnToOriginalPosition()
    {
        Vector3 direction = (originalPosition - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        // Ensure the zombie is facing the right direction
        HandleRotate();
        animator.SetBool("isWalking", true); // Ensure walking animation is playing

        // Check if the enemy has reached the original position
        if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero; // Stop movement
            animator.SetBool("isWalking", false); // Stop walking animation
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * movementSpeed;
        HandleRotate(); // Ensure the enemy is facing the right direction
        animator.SetBool("isWalking", true); // Trigger walking animation
    }

    private bool CheckIfGrounded()
    {
        Collider2D collider2D =
            Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0.0f, groundLayer);
        return collider2D != null;
    }

    private bool CheckIfNearEdge()
    {
        // Cast a ray downward to check if there's ground directly below
        if (!Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundLayer))
        {
            return true; // No ground below, assume edge
        }

        // Cast rays forward to check for ground in front of the enemy
        Vector2 forwardDirection = transform.right; // Assuming right is forward for 2D
        if (!Physics2D.Raycast(transform.position, forwardDirection, edgeDetectionDistance, groundLayer))
        {
            return true; // No ground detected in front, assume edge
        }

        return false;
    }

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking"); // Start attack animation

        // Wait for the attack animation to complete
        yield return new WaitForSeconds(1.0f); // Adjust based on your attack animation duration

        // Apply melee damage
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die(); // Trigger death sequence
        }
    }

    public void Die()
    {
        animator.SetTrigger("isDead"); // Trigger death animation
        rb.velocity = Vector2.zero; // Stop movement
        Destroy(gameObject, 1f); // Delay destruction to allow animation to play
    }
}
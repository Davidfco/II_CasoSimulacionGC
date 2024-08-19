using System.Collections;
using UnityEngine;

public class EnemyController2D : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [SerializeField] private float meleeDamage;
    [SerializeField] private float aggroRange;
    [SerializeField] private float returnRange;

    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private Transform player;

    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask ignoreLayer;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float edgeDetectionDistance;
    [SerializeField] private float meleeAttackRange;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 1, 0);

   
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private Transform ammoDropPosition; // Position for ammo drop

    [SerializeField]
    private GameObject medKitPrefab; // Prefab for med kit drop
    [SerializeField]
    private float healingAmount; // Amount of health restored by the med kit
    [SerializeField] private Transform medKitDropPosition; // Position for ammo drop


    private bool isAggro = false;
    private Vector3 originalPosition;
    private Rigidbody2D rb;
    private bool isAttacking = false;

    private GameObject healthBarenemiesInstance;

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0.0f, groundLayer);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        originalPosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            healthBarenemiesInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            RectTransform healthBarRectTransform = healthBarenemiesInstance.GetComponent<RectTransform>();
            healthBarRectTransform.SetParent(transform);
            healthBarRectTransform.localPosition = healthBarOffset;
            healthBarRectTransform.localScale = Vector3.one;

            HealthBarEnemies healthBarComponent = healthBarenemiesInstance.GetComponent<HealthBarEnemies>();
            if (healthBarComponent != null)
            {
                healthBarComponent.TakeDamage(-currentHealth);
            }
        }
    }

    void Update()
    {
        if (!IsGrounded())
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

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

        if (isAggro && IsGrounded())
        {
            if (distanceToPlayer <= meleeAttackRange)
            {
                if (!isAttacking)
                {
                    StartCoroutine(MeleeAttack());
                }
            }
            else
            {
                if (CheckIfNearEdge())
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
                else
                {
                    ChasePlayer();
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
            {
                ReturnToOriginalPosition();
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        bool isNearEdge = CheckIfNearEdge();
        if (isNearEdge)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (healthBarenemiesInstance != null)
        {
            RectTransform healthBarRectTransform = healthBarenemiesInstance.GetComponent<RectTransform>();
            healthBarRectTransform.position = new Vector3(transform.position.x, transform.position.y, healthBarOffset.z) + healthBarOffset;
        }
    }

    private void HandleRotate()
    {
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
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void ReturnToOriginalPosition()
    {
        Vector3 direction = (originalPosition - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        HandleRotate();
        animator.SetBool("isWalking", true);

        if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        HandleRotate();
        animator.SetBool("isWalking", true);
    }

    private bool CheckIfNearEdge()
    {
        int edgeDetectionMask = ~ignoreLayer;

        if (!Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, edgeDetectionMask))
        {
            return true;
        }

        Vector2 forwardDirection = transform.right;
        if (!Physics2D.Raycast(transform.position, forwardDirection, edgeDetectionDistance, edgeDetectionMask))
        {
            return true;
        }

        return false;
    }

    private IEnumerator MeleeAttack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(1.0f);

        if (Vector3.Distance(transform.position, player.position) <= meleeAttackRange)
        {
            CharacterController2D playerController = player.GetComponent<CharacterController2D>();
            if (playerController != null)
            {
                playerController.TakeDamage(meleeDamage);
            }
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Zombie Current Health: {currentHealth}");

        if (healthBarenemiesInstance != null)
        {
            HealthBarEnemies healthBarComponent = healthBarenemiesInstance.GetComponent<HealthBarEnemies>();
            if (healthBarComponent != null)
            {
                healthBarComponent.TakeDamage(damage);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("isDead");
        rb.velocity = Vector2.zero;
        DropItems();
        Destroy(gameObject, 1f);
    }

    private void DropItems()
    {
        // Define the probability of dropping a med kit vs ammo
        float dropChance = Random.value; // Random value between 0.0 and 1.0

        // 50/50 chance to drop either a med kit or ammo
        if (dropChance < 0.5f)
        {
            if (ammoPrefab != null)
            {
                Instantiate(ammoPrefab, transform.position, Quaternion.identity);
            }
        }
        else
        {
            if (medKitPrefab != null)
            {
                Instantiate(medKitPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
    private int ANIMATION_SPEED;
    private int ANIMATION_FORCE;
    private int ANIMATION_FALL;
    private int ANIMATION_PUNCH;
    private int ANIMATION_SUPER;
    private int ANIMATION_DIE;

    [SerializeField]
    private float maxHealth; // Maximum health of the player
    [SerializeField]
    private float currentHealth; // Current health of the player

    [Header("Movement")]
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float gravityMultiplier;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private Vector2 groundCheckSize;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool isFacingRight;

    [Header("Attack")]
    [SerializeField]
    private Transform punchPoint;

    [SerializeField]
    private float punchRadius;

    [SerializeField]
    private LayerMask attackMask;

    [SerializeField]
    private float dieDelay;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private Transform projectilePoint;

    [SerializeField]
    private float projectileLifeTime;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private float _inputX;
    private float _gravityY;
    private float _velocityY;

    private bool _isGrounded;
    private bool _isJumpPressed;
    private bool _isJumping;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();

        _gravityY = Physics2D.gravity.y;

        ANIMATION_SPEED = Animator.StringToHash("speed");
        ANIMATION_FORCE = Animator.StringToHash("force");
        ANIMATION_FALL = Animator.StringToHash("fall");
        ANIMATION_PUNCH = Animator.StringToHash("punch");
        ANIMATION_SUPER = Animator.StringToHash("super");
        ANIMATION_DIE = Animator.StringToHash("die");

        currentHealth = maxHealth; // Initialize current health
    }

    private void Start()
    {
        HandleGrounded();
    }

    private void Update()
    {
        HandleGravity();
        HandleInputMove();
    }

    private void FixedUpdate()
    {
        HandleJump();
        HandleRotate();
        HandleMove();
    }

    private void HandleGrounded()
    {
        _isGrounded = IsGrounded();
        if (!_isGrounded)
        {
            StartCoroutine(WaitForGroundedCoroutine());
        }
    }

    private void HandleGravity()
    {
        if (_isGrounded)
        {
            if (_velocityY < -1.0F)
            {
                _velocityY = -1.0F;
            }

            HandleInputJump();
        }
    }

    private void HandleInputJump()
    {
        _isJumpPressed = Input.GetButton("Jump");
    }

    private void HandleInputMove()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
    }

    private void HandleJump()
    {
        if (_isJumpPressed)
        {
            _isJumpPressed = false;
            _isGrounded = false;
            _isJumping = true;

            _velocityY = jumpForce;

            _animator.SetTrigger(ANIMATION_FORCE);

            StartCoroutine(WaitForGroundedCoroutine());
        }
        else if (!_isGrounded)
        {
            _velocityY += _gravityY * gravityMultiplier * Time.fixedDeltaTime;
            if (!_isJumping)
            {
                _animator.SetTrigger(ANIMATION_FALL);
            }
        }
        else if (_isGrounded)
        {
            if (_velocityY >= 0.0F)
            {
                _velocityY = -1.0F;
            }
            else
            {
                HandleGrounded();
            }

            _isJumping = false;
        }
    }

    private void HandleMove()
    {
        float speed = _inputX != 0.0F ? 1.0F : 0.0F;
        float animatorSpeed = _animator.GetFloat(ANIMATION_SPEED);

        if (speed != animatorSpeed)
        {
            _animator.SetFloat(ANIMATION_SPEED, speed);
        }

        Vector2 velocity = new Vector2(_inputX, 0.0F) * walkSpeed * Time.fixedDeltaTime;
        velocity.y = _velocityY;

        _rigidbody.velocity = velocity;
    }

    private void HandleRotate()
    {
        if (_inputX == 0.0F)
        {
            return;
        }

        bool facingRight = _inputX > 0.0F;
        if (isFacingRight != facingRight)
        {
            isFacingRight = facingRight;
            transform.Rotate(0.0F, 180.0F, 0.0F);
        }
    }

    private bool IsGrounded()
    {
        Collider2D collider2D =
            Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0.0F, groundMask);
        return collider2D != null;
    }

    private IEnumerator WaitForGroundedCoroutine()
    {
        yield return new WaitUntil(() => !IsGrounded());
        yield return new WaitUntil(() => IsGrounded());
        _isGrounded = true;
    }

  

    public void Punch()
    {
        _animator.SetTrigger(ANIMATION_PUNCH);
    }

    public void Punch(float damage, bool isPercentage)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(punchPoint.position, punchRadius, attackMask);

        foreach (Collider2D collider in colliders)
        {
            DamageableController controller = collider.GetComponent<DamageableController>();
            if (controller == null)
            {
                continue;
            }

            controller.TakeDamage(damage, isPercentage);
        }
    }

    public void Super()
    {
        _animator.SetTrigger(ANIMATION_SUPER);
    }

    public void Super(float damage, bool isPercentage)
    {
        GameObject projectile = Instantiate(projectilePrefab, projectilePoint.position, transform.rotation);
        ProjectileController controller = projectile.GetComponent<ProjectileController>();
        controller.Go(damage, isPercentage);
        Destroy(projectile, projectileLifeTime);
    }

    public void Die()
    {
     StartCoroutine(DieCoroutine());
      
    }

   private IEnumerator DieCoroutine()
     {
        _animator.SetTrigger("isDead"); // Trigger death animation
        _rigidbody.velocity = Vector2.zero; // Stop movement
        yield return new WaitForSeconds(dieDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  
    }

    // Method to handle taking damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Current Health: {currentHealth}"); // Log the current health value
        if (currentHealth <= 0)
        {
           
            Die(); // Trigger death sequence
        }
    }
}

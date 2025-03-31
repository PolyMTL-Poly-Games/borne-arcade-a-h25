using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Setup")]
    // A mask determining what is terrain to the character
    // Use layer 8 'Terrain', not tag!
    [SerializeField] private LayerMask whatIsTerrain = 1 << 8;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private GameObject healEffectPrefab;

    const float DETECTION_RADIUS = .2f;
    private bool isTouchingWall;
    private bool isGrounded;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 18f;
    [SerializeField] private float wallPushForce = 10f;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private float controlLockTime = 0.2f;
    [SerializeField] private float enemyKnockbackForce = 30f;
    [SerializeField] private float enemyKnockbackUpwardForce = 20f;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    private bool hasControl = true;
    public int jumpCount = 0;

    // Health
    public HealthBar healthBar;
    private int health = 5;
    private int maxHealth = 5;
    private int essenceCount = 0;
    private int essenceThreshold = 3;

    // Respawn at boss
    static public bool isAtBoss = false;
    public GameObject bossFightRespawnPoint;

    // Input system variables
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool jumpPressed;

    // Audio
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip stompSound;
    public AudioClip jumpSound;

    void Awake()
    {
        healthBar.SetHealth(maxHealth);

        wallCheck = transform.Find("WallCheck");
        groundCheck = transform.Find("GroundCheck");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Initialize Input Actions
        inputActions = new PlayerInputActions();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed += ctx => jumpPressed = true;
        inputActions.Player.Jump.canceled += ctx => jumpPressed = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        CheckHP();
        Move();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        isTouchingWall = false;
        CheckTerrain();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isAtBoss && bossFightRespawnPoint != null)
            transform.position = bossFightRespawnPoint.transform.position;
    }

    private void CheckHP()
    {
        if (health <= 0)
        {
            AudioManagerController.instance?.PlaySound(deathSound);
            AudioManagerController.instance?.ResetTrack();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    private void Move()
    {
        if (hasControl)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            HandleFacing(moveInput.x);
        }

        // If the player can jump...
        if (isGrounded && jumpPressed)
        {
            Jump();
        }
        else if (!isGrounded && jumpPressed)
        {
            if (isTouchingWall)
            {
                WallJump();
            }
            else if (jumpCount < maxJumpCount)
            {
                Jump();
                anim.SetTrigger("wallJump");
            }
        }

        // Reset jumpPressed after processing to prevent multiple jumps with single press
        jumpPressed = false;
    }

    private void Jump()
    {
        isGrounded = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        ++jumpCount;
        AudioManagerController.instance?.PlaySound(jumpSound);
    }

    private void WallJump()
    {
        hasControl = false;
        if (isFacingRight)
            rb.linearVelocity = new Vector2(-wallPushForce, jumpForce);
        else
            rb.linearVelocity = new Vector2(wallPushForce, jumpForce);

        AudioManagerController.instance?.PlaySound(jumpSound);

        // Lock movement for a short duration to prevent player from sticking on wall when maintaining input
        Invoke("AllowControl", controlLockTime);
        jumpCount = 0;
    }

    private void AllowControl()
    {
        hasControl = true;
    }

    private void HandleFacing(float move)
    {
        if (move > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (move < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void CheckTerrain()
    {
        // To prevent jumpCount = 0 when we just started jumping
        if (rb.linearVelocity.y <= 0)
        {
            // Ground check
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, DETECTION_RADIUS, whatIsTerrain);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    isGrounded = true;
                    jumpCount = 0;
                }
            }
        }

        // Wall check
        Collider2D[] walls = Physics2D.OverlapCircleAll(wallCheck.position, DETECTION_RADIUS, whatIsTerrain);
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i].gameObject != gameObject)
            {
                isTouchingWall = true;
            }
        }
    }

    // For Debugging: we can see detection radius
    private void OnDrawGizmos()
    {
        // Draw a sphere to visualize groundRadius
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, DETECTION_RADIUS);
        }

        // Draw a sphere to visualize wallRadius
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, DETECTION_RADIUS);
        }
    }

    private void UpdateAnimations()
    {
        anim.SetFloat("hSpeed", Mathf.Abs(moveInput.x));
        anim.SetFloat("vSpeed", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isHoldingWall", !isGrounded && isTouchingWall && Mathf.Abs(moveInput.x) > 0);
    }

    // Used by enemies and projectiles
    public void Hurt(GameObject danger)
    {
        Vector2 knockbackDirection = (transform.position.x > danger.transform.position.x) ? Vector2.right : Vector2.left;
        Vector2 knockback = new Vector2(knockbackDirection.x * enemyKnockbackForce / 2, enemyKnockbackUpwardForce / 2);

        rb.linearVelocity = knockback;

        hasControl = false;
        Invoke("AllowControl", controlLockTime);


        anim.SetTrigger("hit");
        health--;
        healthBar.SetHealth(health);

        if (health != 0)
            AudioManagerController.instance?.PlaySound(hurtSound);
    }

    public void KillPlayer()
    {
        health = 0;
    }

    public void PlayStompSound()
    {
        AudioManagerController.instance?.PlaySound(stompSound);
    }

    public void EssenceDrain()
    {
        essenceCount++;
        if (essenceCount >= essenceThreshold)
        {
            Heal();
            essenceCount = 0;
        }
    }

    private void Heal()
    {
        if (health < maxHealth)
        {
            health++;
            healthBar.SetHealth(health);
        }

        if (healEffectPrefab != null)
        {
            GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
            healEffect.transform.SetParent(transform);

            ParticleSystem particleSystem = healEffect.GetComponent<ParticleSystem>();
            Destroy(healEffect, particleSystem.main.duration);
        }
    }
}
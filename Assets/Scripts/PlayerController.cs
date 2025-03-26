using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Setup")]
    // A mask determining what is terrain to the character
    // Use layer 8 'Terrain', not tag!
    [SerializeField] private LayerMask whatIsTerrain = 1 << 8;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform groundCheck;

    const float DETECTION_RADIUS = .2f;
    private bool isTouchingWall;
    private bool isGrounded;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 10f;
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
    private int jumpCount = 0;

    private int health = 5;
    private int maxHealth = 5;
    private bool died = false;
    private float respawnTime = 2f;

    // Input system variables
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool jumpPressed;

    void Awake()
    {
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
    }

    private void OnDisable()
    {
        inputActions.Disable();
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

    private void OnGUI()
    {
        if (died)
        {
            // Not working yet
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 50, 50), "You died");
        }
    }

    private void CheckHP()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            rb.position = new Vector3((float)-5.9, (float)-1.81, 0);
            rb.linearVelocity = new Vector2(0f, 0f);
            health = maxHealth;
            hasControl = false;
            died = true;
            Invoke("AllowControl", respawnTime);
        }
        died = false;
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
    }

    private void WallJump()
    {
        hasControl = false;
        if (isFacingRight)
            rb.linearVelocity = new Vector2(-wallPushForce, jumpForce);
        else
            rb.linearVelocity = new Vector2(wallPushForce, jumpForce);

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
        Vector2 knockback = new Vector2(knockbackDirection.x * enemyKnockbackForce, enemyKnockbackUpwardForce);

        rb.linearVelocity = knockback;

        hasControl = false;
        Invoke("AllowControl", controlLockTime);

        anim.SetTrigger("hit");
        health--;
    }
}
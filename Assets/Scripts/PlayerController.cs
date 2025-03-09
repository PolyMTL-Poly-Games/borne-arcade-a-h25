using System;
using UnityEngine;

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
    [SerializeField] private float wallJumpLockTime = 0.2f;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    private bool hasAirControl = true;
    private int jumpCount = 0;

    void Awake()
    {
        wallCheck = transform.Find("WallCheck");
        groundCheck = transform.Find("GroundCheck");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        bool hasPressedJump = Input.GetButtonDown("Jump");

        Move(moveInput, hasPressedJump);
        UpdateAnimations(moveInput);
    }

    void FixedUpdate()
    {
        isGrounded = false;
        isTouchingWall = false;

        CheckTerrain();
    }

    private void Move(float moveInput, bool hasPressedJump)
    {
        if (isGrounded || hasAirControl)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
            HandleFacing(moveInput);
        }

        // If the player can jump...
        if (isGrounded && hasPressedJump)
        {
            Jump();
        }
        else if (!isGrounded && hasPressedJump)
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
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        ++jumpCount;
    }

    private void WallJump()
    {
        hasAirControl = false;
        if (isFacingRight)
            rb.linearVelocity = new Vector2(-wallPushForce, jumpForce);
        else
            rb.linearVelocity = new Vector2(wallPushForce, jumpForce);

        // Lock movement for a short duration to prevent player from sticking on wall when maintaining input
        Invoke("AllowAirControl", wallJumpLockTime);
        jumpCount = 0;
    }

    private void AllowAirControl()
    {
        hasAirControl = true;
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

    private void UpdateAnimations(float moveInput)
    {
        anim.SetFloat("hSpeed", Math.Abs(moveInput));
        anim.SetFloat("vSpeed", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isHoldingWall", !isGrounded && isTouchingWall && Math.Abs(moveInput) > 0);
    }
}

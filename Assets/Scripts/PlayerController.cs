using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 18f;
    public float wallJumpPushForce = 12f;
    public float wallJumpUpwardForce = 22f;
    public float wallJumpLockTime = 0.1f;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canWallJump;
    private bool isFacingRight = true;
    private Vector2 wallNormalDirection;
    private float wallJumpLockTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (wallJumpLockTimer > 0)
        {
            wallJumpLockTimer -= Time.deltaTime;
        }

        float moveInput = Input.GetAxis("Horizontal");

        HandleMovementAnimation(moveInput);

        if (wallJumpLockTimer <= 0)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (canWallJump)
            {
                PerformWallJump();
            }
        }
    }

    void PerformWallJump()
    {
        float jumpDirection = wallNormalDirection.x > 0 ? 1 : -1;

        rb.linearVelocity = new Vector2(jumpDirection * wallJumpPushForce, wallJumpUpwardForce);

        if ((jumpDirection > 0 && !isFacingRight) || (jumpDirection < 0 && isFacingRight))
        {
            Flip();
        }

        canWallJump = false;
        wallJumpLockTimer = wallJumpLockTime; // Lock movement pendant  un moment pour empêcher joueur de stick sur mur
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canWallJump = false;
        }
        else if (collision.gameObject.CompareTag("Wall") && !isGrounded)
        {
            canWallJump = true;
            wallNormalDirection = collision.GetContact(0).normal;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            canWallJump = false;
        }
    }

    private void HandleMovementAnimation(float moveInput)
    {
        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            Flip();
        }

        animator.SetFloat("isRunning", Math.Abs(moveInput));
        animator.SetBool("isFalling", rb.linearVelocity.y < 0);
        animator.SetBool("isJumping", !isGrounded);
        animator.SetBool("isHoldingWall", canWallJump && Math.Abs(moveInput) > 0);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}

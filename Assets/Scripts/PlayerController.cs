using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveSpeed = 10f;      // Horizontal movement speed
    public float jumpForce = 18f;    // Force applied for jumping

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canWallJump;
    private Vector2 wallNormalDirection;
    private bool isFacingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        handleMovementAnimation(moveInput);

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (canWallJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce - moveInput);
            }
        }
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
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            canWallJump = false;
        }
    }

    // Je dois résoudre des bugs
    private void handleMovementAnimation(float moveInput)
    {
        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        animator.SetFloat("isRunning", Math.Abs(moveInput));
        animator.SetBool("isFalling", rb.linearVelocity.y < 0);
        animator.SetBool("isJumping", !isGrounded);
        Debug.Log(moveInput);
        animator.SetBool("isHoldingWall", canWallJump && Math.Abs(moveInput) > 0);
    }
}

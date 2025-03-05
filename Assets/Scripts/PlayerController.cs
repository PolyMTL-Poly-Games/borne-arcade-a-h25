using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveSpeed = 10f;      // Horizontal movement speed
    public float jumpForce = 20f;    // Force applied for jumping

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canWallJump;
    private Vector2 wallNormalDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
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
        else if (collision.gameObject.CompareTag("Wall"))
        {
            canWallJump = true;
            isGrounded = false;
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
}

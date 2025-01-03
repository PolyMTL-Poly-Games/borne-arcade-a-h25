using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    [SerializeField]
    private float followSpeed = 2f;
    [SerializeField]
    private float detectionRange = 8f;
    public Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;
    public float waitTime = 1f; // Wait time between jumps
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float jumpForce = 5f; // Jump force for the slime
    private bool isGrounded = true; // Check if the slime is grounded
    private float jumpCooldown = 1f; // Cooldown between jumps
    private float lastJumpTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
            JumpTowardsPlayer();
        }
    }

    void FollowPlayer()
    {
        // Determine the direction to the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Flip the slime's sprite to face the player
        if (direction.x > 0)
        {
            spriteRenderer.flipX = true; // Face right
        }
        else
        {
            spriteRenderer.flipX = false; // Face left
        }
    }

    void JumpTowardsPlayer()
    {
        if (isGrounded && Time.time > lastJumpTime + jumpCooldown)
        {
            // Jump towards the player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * followSpeed, jumpForce);

            // Set grounded to false and update the jump time
            isGrounded = false;
            lastJumpTime = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Reset grounded when touching the ground
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // Kill the slime when the player jumps on it
            if (collision.transform.position.y > transform.position.y + 0.5f)
            {
                Die();
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 10f); // Bounce player up
            }
            else
            {
                // Damage the player
                Debug.Log("Player hit by slime!");
            }
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("GettingHit");
        Destroy(gameObject, 0.5f); // Delay to let the animation play
    }
}

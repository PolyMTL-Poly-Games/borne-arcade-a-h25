using UnityEngine;

public class SlimeBehavior : EnemyController
{
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float jumpForce = 10f;

    private Transform player;
    private Rigidbody2D rb;
    public float waitTime = 1f;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded = true;
    private float jumpCooldown = 1f;
    private float lastJumpTime;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            // Reset grounded when touching the ground
            isGrounded = true;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere to visualize detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

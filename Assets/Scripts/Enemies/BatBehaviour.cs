using UnityEngine;

public class BatBehavior : MonoBehaviour
{
    [SerializeField]
    private float followSpeed = 2f;
    [SerializeField]
    private float detectionRange = 8f;
    public Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;
    private bool isWaiting = false;
    public float waitTime = 1f;
    private SpriteRenderer spriteRenderer;

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
        }
    }

    void FollowPlayer()
    {
        animator.Play("Flying");

        // Determine the direction to the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Set the velocity
        rb.linearVelocity = direction * followSpeed;

        // Flip the bat's sprite to face the player
        if (direction.x > 0)
        {
            spriteRenderer.flipX = true; // Face right
        }
        else
        {
            spriteRenderer.flipX = false; // Face left
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kill the bat when the player jumps on it
            if (collision.transform.position.y > transform.position.y + 0.5f)
            {
                Die();
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 10f); // Bounce player up
            }
            else
            {
                // Damage the player
                Debug.Log("Player hit by bat!");
            }
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.Play("GettingHit");
        Destroy(gameObject, 0.25f); // Delay to let the animation play
    }
}

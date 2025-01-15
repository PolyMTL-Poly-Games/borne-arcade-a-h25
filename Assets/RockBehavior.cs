using UnityEngine;
using System.Collections;

public class RockBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; // Movement speed
    [SerializeField] private Transform leftEdge; // Left patrol edge
    [SerializeField] private Transform rightEdge; // Right patrol edge
    public Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float direction = 1f; // Moving direction (1 for right, -1 for left)

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // Flip the sprite based on direction
        spriteRenderer.flipX = direction > 0;
        // Move the bee
        transform.Translate(Vector2.right * moveSpeed * direction * Time.deltaTime);

        // Reverse direction if reaching patrol edges
        if (transform.position.x >= rightEdge.position.x && direction > 0)
        {
            direction = -1f; // Start moving left
        }
        else if (transform.position.x <= leftEdge.position.x && direction < 0)
        {
            direction = 1f; // Start moving right
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the player is jumping on top of the bee
            if (collision.transform.position.y > transform.position.y + 0.5f)
            {
                Die();
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 10f); // Bounce player up
            }
            else
            {
                // Damage the player
                Debug.Log("Player hit by bee!");
            }
        }
    }

    private void Die()
    {
        animator.SetTrigger("Hit");
        // Disable bee after hit animation
        Destroy(gameObject, 0.5f); // Delay to let the animation play
    }
}

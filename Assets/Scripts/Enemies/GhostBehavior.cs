using UnityEngine;
using System.Collections;

public class GhostBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; // Movement speed
    [SerializeField] private float disappearDuration = 3f; // Time ghost stays invisible
    [SerializeField] private float appearDuration = 5f; // Time ghost stays visible
    [SerializeField] private Transform leftEdge; // Left patrol edge
    [SerializeField] private Transform rightEdge; // Right patrol edge

    private bool isVisible = true; // State of the ghost
    private SpriteRenderer spriteRenderer;
    private Collider2D ghostCollider;
    private Animator animator;
    private float direction = 1f; // Moving direction (1 for right, -1 for left)

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ghostCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(CycleVisibility());
    }

    private void Update()
    {
        if (isVisible)
        {
            Move();
        }
    }

    private void Move()
    {
        // Flip the sprite based on direction
        spriteRenderer.flipX = direction > 0;

        // Move the ghost
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

    private IEnumerator CycleVisibility()
    {
        while (true)
        {
            // Disappear
            isVisible = false;
            animator.SetTrigger("Disappear");
            yield return new WaitForSeconds(1f);
            ghostCollider.enabled = false;
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(disappearDuration);

            // Reappear
            isVisible = true;
            ghostCollider.enabled = true;
            spriteRenderer.enabled = true;
            animator.SetTrigger("Appear");
            yield return new WaitForSeconds(appearDuration);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isVisible)
        {
            // Check if the player is jumping on top of the ghost
            if (collision.transform.position.y > transform.position.y + 0.5f)
            {
                Die();
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 10f); // Bounce player up
            }
            else
            {
                // Damage the player
                Debug.Log("Player hit by ghost!");
            }
        }
    }

    private void Die()
    {
        animator.SetTrigger("Hit");
        // Disable ghost after hit animation
        Destroy(gameObject, 0.5f); // Delay to let the animation play
    }
}

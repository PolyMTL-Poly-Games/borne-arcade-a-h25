using UnityEngine;
using System.Collections;

public class GhostBehavior : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f; // Movement speed
    [SerializeField]
    private float disappearDuration = 3f; // Time ghost stays invisible
    [SerializeField]
    private float appearDuration = 5f; // Time ghost stays visible

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
        if (direction > 0)
        {
            spriteRenderer.flipX = true; // Face right
        }
        else
        {
            spriteRenderer.flipX = false; // Face left
        }

        transform.Translate(Vector2.right * moveSpeed * direction * Time.deltaTime);

        // logic to flip direction at edges
        RaycastHit2D edgeCheck = Physics2D.Raycast(transform.position + Vector3.right * direction, Vector2.down, 1f);
        if (!edgeCheck.collider)
        {
            direction *= -1; // Flip direction
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

using UnityEngine;

public class BatBehavior : EnemyController
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float detectionRange = 8f;

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // animator from the parent class
        animator.SetTrigger("flying");

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

    private void OnDrawGizmos()
    {
        // Draw a sphere to visualize detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

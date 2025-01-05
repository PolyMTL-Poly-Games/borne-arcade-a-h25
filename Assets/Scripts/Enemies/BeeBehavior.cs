using UnityEngine;
using System.Collections;

public class BeeBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; // Movement speed
    [SerializeField] private Transform leftEdge; // Left patrol edge
    [SerializeField] private Transform rightEdge; // Right patrol edge
    [SerializeField] private float detectionRange = 8f;
    private bool canAttack = true;
    private float lastAttackTime = 0f;
    [SerializeField] private GameObject stingPrefab;
    private Transform stingSpawnPoint;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float stingSpeed = 7.5f;
    public Transform player;
    private Animator animator;
    private float direction = 1f; // Moving direction (1 for right, -1 for left)

    private void Start()
    {
        animator = GetComponent<Animator>();
        stingSpawnPoint = transform.GetChild(0);
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        CheckAttackCooldown();
        Move();
        if (distanceToPlayer <= detectionRange)
        {
            if (canAttack)
            {
                animator.SetTrigger("Attack");
                StartCoroutine(WaitAndShoot());
                canAttack = false;
                lastAttackTime = Time.time;
            }
        }
    }

    private IEnumerator WaitAndShoot()
    {
        yield return new WaitForSeconds(1f);
        Shoot();
    }

    private void Move()
    {
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

    private void Shoot()
    {
        GameObject stingInstance = Instantiate(stingPrefab, stingSpawnPoint.position, stingSpawnPoint.rotation);
        Rigidbody2D rb = stingInstance.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(0, -stingSpeed);
        Destroy(stingInstance, 3f);
    }
    private void CheckAttackCooldown()
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            canAttack = true;
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

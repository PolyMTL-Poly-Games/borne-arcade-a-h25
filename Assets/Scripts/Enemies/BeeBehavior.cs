using UnityEngine;
using System.Collections;

public class BeeBehavior : EnemyController
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private GameObject stingPrefab;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float stingSpeed = 7.5f;
    [SerializeField] private float detectionRange = 8f;

    private Transform player;
    private Transform stingSpawnPoint;
    private Transform leftEdge;
    private Transform rightEdge;
    private bool canAttack = true;
    private float lastAttackTime = 0f;
    private float direction = 1f;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player").transform;
        stingSpawnPoint = transform.GetChild(0);
        leftEdge = transform.parent.GetChild(1);
        rightEdge = transform.parent.GetChild(2);
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
                animator.SetTrigger("attack");
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

    private void OnDrawGizmos()
    {
        // Draw a sphere to visualize detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

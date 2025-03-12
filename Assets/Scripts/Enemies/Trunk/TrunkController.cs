using Unity.VisualScripting;
using UnityEngine;

public class TrunkController : EnemyController
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float bulletSpeed = 7.5f;
    [SerializeField] private bool isShootingOnSight = true;

    private Transform bulletSpawnPoint;
    private bool canAttack = true;
    private float lastAttackTime = 0f;
    private int direction;

    protected override void Awake()
    {
        base.Awake();
        bulletSpawnPoint = transform.GetChild(0);
    }

    void Start()
    {
        direction = transform.localScale.x > 0 ? -1 : 1;
    }

    void Update()
    {
        CheckAttackCooldown();
    }

    // Note: Faut mettre Sleeping Mode à Never Sleep dans Rigidbody 2D du joueur
    private void CheckAttackCooldown()
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            canAttack = true;
        }

        if (!isShootingOnSight)
        {
            Attack();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isShootingOnSight && other.CompareTag("Player"))
        {
            Attack();
        }
    }

    void Attack()
    {
        if (canAttack)
        {
            animator.SetTrigger("attack");
            Shoot();
            canAttack = false;
            lastAttackTime = Time.time;
        }
    }

    void Shoot()
    {
        GameObject bulletInstance = Instantiate(
            bulletPrefab,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation
        );

        if (transform.localScale.x < 0)
        {
            Vector3 scale = bulletInstance.transform.localScale;
            scale.x *= -1;
            bulletInstance.transform.localScale = scale;
        }

        Rigidbody2D rb = bulletInstance.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction * bulletSpeed, 0);
    }
}

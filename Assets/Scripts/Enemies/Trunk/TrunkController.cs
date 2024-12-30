using Unity.VisualScripting;
using UnityEngine;

public class TrunkController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float bulletSpeed = 7.5f;
    [SerializeField] private float bounceForce = 15f;

    private Animator animator;
    private Transform bulletSpawnPoint;
    private bool canAttack = true;
    private float lastAttackTime = 0f;
    private bool isHit = false;
    private int direction;

    void Awake()
    {
        animator = GetComponent<Animator>();
        bulletSpawnPoint = transform.GetChild(0);
    }

    void Start()
    {
        direction = transform.localScale.x > 0 ? -1 : 1;

    }

    void Update()
    {
        if (isHit) return;

        CheckAttackCooldown();
    }

    // Note: Faut mettre Sleeping Mode à Never Sleep dans Rigidbody 2D du joueur
    private void CheckAttackCooldown()
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            canAttack = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (canAttack)
            {
                animator.SetTrigger("isAttacking");
                Shoot();
                canAttack = false;
                lastAttackTime = Time.time;
            }
        }
    }

    void Shoot()
    {
        GameObject bulletInstance = Instantiate(
            bulletPrefab,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation
        );


        Rigidbody2D rb = bulletInstance.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction * bulletSpeed, 0);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.relativeVelocity.y < 0)
            {
                BouncePlayer(other);
                HurtMushroom();
            }
        }
    }

    private void BouncePlayer(Collision2D other)
    {
        Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
    }

    private void HurtMushroom()
    {
        isHit = true;
        animator.SetTrigger("isHit");
    }

    // Voir fin d'animation Hit
    public void OnHitAnimationEnd()
    {
        Destroy(gameObject);
    }
}

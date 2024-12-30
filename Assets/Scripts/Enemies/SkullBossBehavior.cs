using UnityEngine;
using System.Collections;

public class SkullBossBehavior : MonoBehaviour
{
    [SerializeField]
    private float enragedDuration = 8f; // Time skull stays enraged
    [SerializeField]
    private float passiveDuration = 5f; // Time skull stays passive
    [SerializeField]
    private GameObject redParticlePrefab; // Prefab for red particle projectile
    [SerializeField]
    private float followSpeed = 2f;
    [SerializeField]
    private float detectionRange = 8f;
    [SerializeField]
    private float shootInterval = 1f; // Time between particle bursts
    [SerializeField]
    private int particleCount = 12; // Number of particles in 360-degree burst
    [SerializeField]
    private float particleSpeed = 5f; // Speed of each particle
    private int hitNumber = 0; // Number of hit for boss
    [SerializeField]
    private int maxLife = 3;
    private bool enragedState;
    public Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        StartCoroutine(CycleState());
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        // Determine the direction to the player
        Vector2 direction = (player.position - transform.position).normalized;
        // Set the velocity
        rb.linearVelocity = direction * followSpeed;

        // Flip the skull's sprite to face the player
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else
        {
            spriteRenderer.flipX = true; // Face left
        }
    }

    private IEnumerator CycleState()
    {
        while (true)
        {
            // Enraged
            enragedState = true;
            animator.SetTrigger("Enraged");
            StartCoroutine(ShootParticles());
            yield return new WaitForSeconds(enragedDuration);

            // Passive
            enragedState = false;
            animator.SetTrigger("Passive");
            yield return new WaitForSeconds(passiveDuration);
        }
    }

    private IEnumerator ShootParticles()
    {
        while (enragedState)
        {
            // Shoot particles in 360-degree spread
            float angleStep = 360f / particleCount;
            float angle = 0f;

            for (int i = 0; i < particleCount; i++)
            {
                // Calculate direction for the particle
                float directionX = Mathf.Cos(angle * Mathf.Deg2Rad);
                float directionY = Mathf.Sin(angle * Mathf.Deg2Rad);
                Vector2 direction = new Vector2(directionX, directionY).normalized;

                // Instantiate the particle and set its velocity
                GameObject particle = Instantiate(redParticlePrefab, transform.position, Quaternion.identity);
                Rigidbody2D particleRb = particle.GetComponent<Rigidbody2D>();
                if (particleRb != null)
                {
                    particleRb.linearVelocity = direction * particleSpeed;
                }

                angle += angleStep;
                Destroy(particle, 4f);
            }

            yield return new WaitForSeconds(shootInterval);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !enragedState)
        {
            // Damage the skull when the player jumps on it
            if (collision.transform.position.y > transform.position.y + 0.5f)
            {
                Die();
                collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(50f, 30f); // Bounce player up
            }
            else
            {
                // Damage the player
                Debug.Log("Player hit by skull!");
            }
        }
    }

    private void Die()
    {
        hitNumber++;
        animator.SetTrigger("Hit");
        if (hitNumber == maxLife)
            // Disable skull after hit animation
            Destroy(gameObject, 0.5f); // Delay to let the animation play
    }
}

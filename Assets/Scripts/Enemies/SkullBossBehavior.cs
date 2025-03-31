using UnityEngine;
using System.Collections;

public class SkullBossBehavior : EnemyController
{
    [SerializeField] private GameObject redParticlePrefab; // Prefab for red particle projectile
    [SerializeField] private GameObject confettiPrefab;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private float enragedDuration = 8f; // Time skull stays enraged
    [SerializeField] private float passiveDuration = 5f; // Time skull stays passive
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float shootInterval = 1f; // Time between particle bursts
    [SerializeField] private int particleCount = 12; // Number of particles in 360-degree burst
    [SerializeField] private float particleSpeed = 5f; // Speed of each particle
    [SerializeField] private GameObject wall;
    [SerializeField] private AudioClip defeatSound;

    private Transform heartContainer;
    private int maxLife = 3;
    private GameObject[] hearts;
    private bool isEnraged;
    private Coroutine cycleCoroutine;

    protected override void Awake()
    {
        base.Awake();
        heartContainer = transform.GetChild(0);
        hitBounceForce = 25f;
    }

    private void Start()
    {
        cycleCoroutine = StartCoroutine(CycleState());
        InitializeHearts();
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
            isEnraged = true;
            animator.SetBool("isEnraged", isEnraged);
            StartCoroutine(ShootParticles());
            yield return new WaitForSeconds(enragedDuration);

            // Passive
            isEnraged = false;
            animator.SetBool("isEnraged", isEnraged);
            yield return new WaitForSeconds(passiveDuration);
        }
    }

    private void RestartCycle()
    {
        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
        }

        cycleCoroutine = StartCoroutine(CycleState());
    }

    private IEnumerator ShootParticles()
    {
        while (isEnraged)
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
                Destroy(particle, 10f);
            }

            yield return new WaitForSeconds(shootInterval);
        }
    }

    protected override void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isEnraged && IsPlayerLanding(other))
            {
                OnDamage(true);
                BouncePlayer(other);
            }
            else if (canDamagePlayer)
            {
                other.gameObject.GetComponent<PlayerController>().Hurt(gameObject);
                StartCoroutine(PauseDamage());
            }
        }
    }

    protected override void OnDamage(bool isColliderStillEnabled = false)
    {
        base.OnDamage(true);
        maxLife--;
        UpdateHearts();
    }

    protected override void OnHitAnimationEnd()
    {
        if (maxLife <= 0)
        {
            base.OnHitAnimationEnd();
            SpawnConfetti();
            wall.SetActive(false);
            AudioManagerController.instance?.audioSource.Stop();
            AudioManagerController.instance?.PlaySound(defeatSound);
        }
        else
        {
            RestartCycle();
        }
    }

    private void InitializeHearts()
    {
        hearts = new GameObject[maxLife];
        float[] xPositions = { -3.17f, -1.6f, 0f };

        for (int i = 0; i < maxLife; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            heart.transform.localPosition = new Vector3(xPositions[i], 0, 0);
            hearts[i] = heart;
        }
    }

    private void UpdateHearts()
    {
        if (maxLife >= 0 && maxLife < hearts.Length)
        {
            hearts[maxLife].GetComponent<Animator>().SetTrigger("lose");
        }
    }

    private void SpawnConfetti()
    {
        Instantiate(confettiPrefab, transform.position, Quaternion.identity);
    }
}

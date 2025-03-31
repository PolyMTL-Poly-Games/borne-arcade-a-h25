using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected float hitBounceForce = 15f;
    [SerializeField] protected float damageCooldown = 1.5f;

    protected PlayerController playerJumpNumber;
    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected bool canDamagePlayer = true;

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerJumpNumber = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // virtual, so the child class can override this method if needed
    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IsPlayerLanding(other))
            {
                OnDamage();
                BouncePlayer(other);
                other.gameObject.GetComponent<PlayerController>().EssenceDrain();
            }
            else if (canDamagePlayer)
            {
                other.gameObject.GetComponent<PlayerController>().Hurt(gameObject);
                StartCoroutine(PauseDamage());
            }
        }
    }

    protected virtual void OnDamage(bool isColliderStillEnabled = false)
    {
        animator.SetTrigger("hit");
        GetComponent<Collider2D>().enabled = isColliderStillEnabled;
    }

    // Voir fin d'animation Hit
    protected virtual void OnHitAnimationEnd()
    {
        Transform parentTransform = transform.parent;
        if (parentTransform != null && parentTransform.tag == "Enemy")
            Destroy(transform.parent.gameObject);
        else
            Destroy(transform.gameObject);
    }

    protected void BouncePlayer(Collision2D other)
    {
        Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, hitBounceForce);
        other.gameObject.GetComponent<PlayerController>().PlayStompSound();
        if (playerJumpNumber.jumpCount > 0)
        {
            playerJumpNumber.jumpCount--;
        }
    }

    protected bool IsPlayerLanding(Collision2D other)
    {
        return other.transform.position.y > transform.position.y;
    }

    protected IEnumerator PauseDamage()
    {
        canDamagePlayer = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamagePlayer = true;
    }
}

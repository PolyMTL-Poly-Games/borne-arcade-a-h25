using System.Data.Common;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected float hitBounceForce = 15f;

    protected PlayerController playerJumpNumber;
    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    private bool collided = false;

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerJumpNumber = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // virtual, so the child class can override this method if needed
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IsPlayerLanding(other))
            {
                OnDamage();
                BouncePlayer(other);
                if (playerJumpNumber.jumpCount > 0)
                {
                    playerJumpNumber.jumpCount--;
                }
            }
            else
            {
                if (!collided)
                {
                    other.gameObject.GetComponent<PlayerController>().Hurt(gameObject);
                }
                collided = !collided;
            }
        }
    }

    protected virtual void OnDamage()
    {
        animator.SetTrigger("hit");
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
    }

    protected bool IsPlayerLanding(Collision2D other)
    {
        return other.relativeVelocity.y < 0 && other.transform.position.y > transform.position.y;
    }
}

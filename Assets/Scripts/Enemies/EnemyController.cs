using System.Data.Common;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float hitBounceForce = 15f;

    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // virtual, so the child class can override this method if needed
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.relativeVelocity.y < 0 && other.transform.position.y > transform.position.y)
            {
                OnDamage();
                BouncePlayer(other);
            }
        }
    }

    protected virtual void OnDamage()
    {
        animator.SetTrigger("hit");
    }

    // Voir fin d'animation Hit
    protected void OnHitAnimationEnd()
    {
        if (transform.parent != null)
            Destroy(transform.parent.gameObject);
        else
            Destroy(transform.gameObject);
    }

    protected void BouncePlayer(Collision2D other)
    {
        Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, hitBounceForce);
    }
}

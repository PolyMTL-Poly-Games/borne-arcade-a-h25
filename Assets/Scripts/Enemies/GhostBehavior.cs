using UnityEngine;
using System.Collections;

public class GhostBehavior : EnemyController
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float disappearDuration = 3f;
    [SerializeField] private float appearDuration = 5f;
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    private bool canMove = true;
    private Collider2D ghostCollider;
    private float direction = 1f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ghostCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(CycleVisibility());
    }

    private void Update()
    {
        if (canMove)
        {
            Move();
        }
    }

    private void Move()
    {
        spriteRenderer.flipX = direction > 0;

        transform.Translate(Vector2.right * moveSpeed * direction * Time.deltaTime);

        if (transform.position.x >= rightEdge.position.x && direction > 0)
        {
            direction = -1f;
        }
        else if (transform.position.x <= leftEdge.position.x && direction < 0)
        {
            direction = 1f;
        }
    }

    private IEnumerator CycleVisibility()
    {
        while (true)
        {
            // Disappear
            canMove = false;
            animator.SetTrigger("disappear");
            yield return new WaitForSeconds(disappearDuration);

            // Reappear
            animator.SetTrigger("reappear");
            yield return new WaitForSeconds(appearDuration);
        }
    }

    // Executed at the end of the disappear animation. See Animation window 
    public void Disappear()
    {
        ghostCollider.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void Reappear()
    {
        ghostCollider.enabled = true;
        spriteRenderer.enabled = true;
        canMove = true;
    }
}

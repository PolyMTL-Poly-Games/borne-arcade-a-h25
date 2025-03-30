using System.Collections;
using UnityEngine;

public class MushroomController : EnemyController
{
    private enum PatrolEdge
    {
        Left,
        Right
    }

    [SerializeField] private PatrolEdge startingTarget = PatrolEdge.Left;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float pauseTime = 1.5f;
    [SerializeField] private float initialDelay = 1f; // Delay before starting patrol to allow explosion
    [SerializeField] private float explosionXRange = 1f;
    [SerializeField] private float explosionYRange = 1f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private bool isGettingBigger = false;

    private Transform patrolEdgeLeft;
    private Transform patrolEdgeRight;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private float pauseTimer = 0f;
    private bool hasStartedPatrol = false;

    protected override void Awake()
    {
        base.Awake();
        patrolEdgeLeft = transform.parent.GetChild(1);
        patrolEdgeRight = transform.parent.GetChild(2);
    }

    private void Start()
    {
        targetPosition = startingTarget == PatrolEdge.Left ? patrolEdgeLeft.position : patrolEdgeRight.position;
        defineFacing();

        StartCoroutine(DelayPatrolStart());
    }

    private void Update()
    {
        if (hasStartedPatrol)
        {
            if (isMoving)
                Patrol();
            else
                Rest();
        }
    }


    private void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetPosition.x, transform.position.y), Time.deltaTime * patrolSpeed);

        float horizontalDistance = Mathf.Abs(transform.position.x - targetPosition.x);
        if (horizontalDistance < 0.1f)
        {
            setMovement(false);
            pauseTimer = pauseTime;
        }
    }

    private void Rest()
    {
        pauseTimer -= Time.deltaTime;
        if (pauseTimer <= 0) RestartPatrol();
    }

    private void RestartPatrol()
    {
        targetPosition = targetPosition == (Vector2)patrolEdgeLeft.position ? patrolEdgeRight.position : patrolEdgeLeft.position;
        defineFacing();

        setMovement(true);
    }

    private void defineFacing()
    {
        Vector2 scale = transform.localScale;
        if (targetPosition == (Vector2)patrolEdgeLeft.position)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    private void setMovement(bool isMoving)
    {
        this.isMoving = isMoving;
        animator.SetBool("moving", isMoving);
    }

    protected override void OnDamage(bool isColliderStillEnabled = false)
    {
        base.OnDamage(true);
    }

    protected override void OnHitAnimationEnd()
    {
        Multiply();
    }

    private void Multiply()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject miniMushroom = Instantiate(transform.parent.gameObject, transform.parent.position, Quaternion.identity);

            float growthFactor = isGettingBigger ? 1.25f : 0.75f;
            miniMushroom.transform.GetChild(0).transform.localScale *= growthFactor;

            Rigidbody2D miniRb = miniMushroom.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>();

            float randomX = Random.Range(-explosionXRange, explosionXRange);
            float randomY = Random.Range(0, explosionYRange);
            miniRb.AddForce(new Vector2(randomX, randomY) * explosionForce, ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }

    private IEnumerator DelayPatrolStart()
    {
        yield return new WaitForSeconds(initialDelay);
        setMovement(true);
        hasStartedPatrol = true;
    }
}
using UnityEngine;

public class MushroomController : EnemyController
{
    private enum PatrolEdge
    {
        Left,
        Right
    }

    [Header("Settings")]
    [SerializeField] private PatrolEdge startingTarget = PatrolEdge.Left;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float pauseTime = 1.5f;

    private Transform patrolEdgeLeft;
    private Transform patrolEdgeRight;
    private Vector2 targetPosition;
    private bool isMoving = true;
    private float pauseTimer = 0f;

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

        setMovement(true);
    }

    private void Update()
    {
        if (isMoving)
            Patrol();
        else
            Rest();
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
}
using UnityEngine;
using System.Collections;

public class RockheadController : MonoBehaviour
{
    [SerializeField] private float groundCheckDistance = 10f;
    [SerializeField] private float dropSpeed = 10f;
    [SerializeField] private float resetSpeed = 2f;
    [SerializeField] private float waitTime = 1f;

    private Animator rockHeadAnimator;
    private Vector3 originalPosition;
    private bool isTriggered = false;
    private bool isReturning = false;

    void Awake()
    {
        rockHeadAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isTriggered)
        {
            transform.position += Vector3.down * dropSpeed * Time.deltaTime;
        }

        if (isReturning)
        {
            // Smoothly return to the original position
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, resetSpeed * Time.deltaTime);

            // Stop returning if back at the original position
            if (transform.position == originalPosition)
            {
                isReturning = false;
            }
        }
    }

    private IEnumerator ResetTrap()
    {
        isTriggered = false;
        yield return new WaitForSeconds(waitTime);
        isReturning = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered && !isReturning)
        {
            isTriggered = true;
            rockHeadAnimator.SetTrigger("TriggerDrop");
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (isTriggered && other.gameObject.CompareTag("Player"))
        {
            Vector2 thwompPosition = transform.position;
            Vector2 playerPosition = other.gameObject.transform.position;

            if (playerPosition.y < thwompPosition.y)
            {
                other.gameObject.GetComponent<PlayerController>().Hurt(gameObject);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            StartCoroutine(ResetTrap());
        }
    }

}

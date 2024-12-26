using UnityEngine;
using System.Collections;

public class ThwompTrap : MonoBehaviour
{
    public Animator rockHeadAnimator; // Reference to the Animator component
    public float dropSpeed = 10f;     // Speed of the trap dropping
    public float resetSpeed = 2f;     // Speed of the trap returning to its original position
    public float waitTime = 1f;       // Time to wait before resetting

    private Vector3 originalPosition; // Original position of the Thwomp
    private bool isTriggered = false;
    private bool isReturning = false;

    void Start()
    {
        originalPosition = transform.position; // Save the original position

        if (rockHeadAnimator == null)
        {
            rockHeadAnimator = GetComponent<Animator>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered && !isReturning)
        {
            isTriggered = true;

            // Trigger the drop animation
            rockHeadAnimator.SetTrigger("TriggerDrop");
        }
    }

    void Update()
    {
        if (isTriggered)
        {
            // Drop the trap
            transform.position += Vector3.down * dropSpeed * Time.deltaTime;

            // Replace this with your ground collision or limit condition
            if (Physics2D.Raycast(transform.position, Vector2.down, 0.1f))
            {
                StartCoroutine(ResetTrap());
            }
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

        // Wait for a moment before resetting
        yield return new WaitForSeconds(waitTime);

        // Start returning the trap to its original position
        isReturning = true;
    }

    // Optional: Triggered by Animation Event at the end of the drop animation
    public void StartReset()
    {
        if (!isReturning)
        {
            StartCoroutine(ResetTrap());
        }
    }
}

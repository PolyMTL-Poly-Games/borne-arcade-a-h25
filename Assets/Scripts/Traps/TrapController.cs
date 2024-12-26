using UnityEngine;

public class TrapController : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private float fire_delay = 2f;
    private void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player has entered the trap area
        if (collision.CompareTag("Player"))
        {
            ActivateTrap();
        }
    }

    private void ActivateTrap()
    {
        // Trigger the "ActivateTrap" animation
        animator.SetBool("ActivateTrap", true);

        // Optionally, start a coroutine to trigger the fire animation after the activate animation finishes
        StartCoroutine(ShowFireAfterDelay(1f)); // Adjust delay based on animation length
    }

    private System.Collections.IEnumerator ShowFireAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Trigger the "ShowFire" animation
        animator.SetBool("ShowFire", true);
        yield return new WaitForSeconds(fire_delay);
        animator.SetBool("ActivateTrap", false);
        animator.SetBool("ShowFire", false);
    }
}

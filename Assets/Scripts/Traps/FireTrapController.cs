using UnityEngine;
using System.Collections;

public class FireTrapController : MonoBehaviour
{
    private Animator animator;
    private float fire_delay = 2f;
    private Transform fireZone;

    void Awake()
    {
        animator = GetComponent<Animator>();
        fireZone = transform.GetChild(1);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateTrap();
        }
    }

    private void ActivateTrap()
    {
        animator.SetBool("ActivateTrap", true);
        StartCoroutine(ShowFireAfterDelay(1f));
    }

    private IEnumerator ShowFireAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("ShowFire", true);
        fireZone.gameObject.SetActive(true);

        yield return new WaitForSeconds(fire_delay);
        animator.SetBool("ActivateTrap", false);
        animator.SetBool("ShowFire", false);
        fireZone.gameObject.SetActive(false);
    }
}

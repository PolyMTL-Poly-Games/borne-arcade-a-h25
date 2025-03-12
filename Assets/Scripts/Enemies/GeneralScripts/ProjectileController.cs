using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Hurt(gameObject);
        }
    }
}

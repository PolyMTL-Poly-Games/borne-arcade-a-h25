using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private bool collidedWithPlayerAlready = false;
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!collidedWithPlayerAlready)
            {
                other.gameObject.GetComponent<PlayerController>().Hurt(gameObject);
            }
            collidedWithPlayerAlready = !collidedWithPlayerAlready;
        }
    }
}

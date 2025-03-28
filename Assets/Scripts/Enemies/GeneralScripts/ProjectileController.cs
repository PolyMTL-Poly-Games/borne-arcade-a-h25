using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private bool collidedWithPlayerAlready = false;
    public bool hasHitPlayer = false;
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!collidedWithPlayerAlready)
            {
                other.gameObject.GetComponent<PlayerController>().Hurt(gameObject);
                hasHitPlayer = true;
                Destroy(gameObject);
            }
            collidedWithPlayerAlready = !collidedWithPlayerAlready;
        }
        else
        {
            hasHitPlayer = false;
        }
    }
}

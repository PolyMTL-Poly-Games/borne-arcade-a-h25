using UnityEngine;

public class TrunkBulletPieceController : MonoBehaviour
{
    [SerializeField] private float pieceDuration = 5f;

    private void Start()
    {
        Destroy(gameObject, pieceDuration);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Ground"))
        {
            Physics2D.IgnoreCollision(other.collider, GetComponent<Collider2D>());
        }
    }
}

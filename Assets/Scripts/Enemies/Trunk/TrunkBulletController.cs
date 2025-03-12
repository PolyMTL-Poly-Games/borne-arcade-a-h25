using UnityEngine;

public class TrunkBulletController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPiecePrefab;
    [SerializeField] private int pieceCount = 5;
    [SerializeField] private float explosionForce = 50f;

    // Note: les ennemis et les bullets s'ignorent grâce au Layer Collision Matrix. Voir Edit > Project Settings > Physics 2D
    private void OnCollisionEnter2D(Collision2D other)
    {
        BreakBullet();
        Destroy(gameObject);
    }

    void BreakBullet()
    {
        for (int i = 0; i < pieceCount; i++)
        {
            GameObject piece = Instantiate(bulletPiecePrefab, transform.position, transform.rotation);
            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
            rb.AddForce(Random.insideUnitCircle * explosionForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-50f, 50f), ForceMode2D.Impulse);
        }
    }
}

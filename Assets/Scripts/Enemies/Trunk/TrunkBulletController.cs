using UnityEngine;

public class TrunkBulletController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPiecePrefab;
    [SerializeField] private int pieceCount = 5;
    [SerializeField] private float explosionRadius = 20f;

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
            rb.AddForce(Random.insideUnitCircle * explosionRadius, ForceMode2D.Impulse);
        }
    }
}

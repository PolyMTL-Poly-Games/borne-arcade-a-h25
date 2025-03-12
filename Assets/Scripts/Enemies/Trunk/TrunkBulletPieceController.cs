using UnityEngine;

public class TrunkBulletPieceController : ProjectileController
{
    [SerializeField] private float pieceDuration = 5f;

    private void Start()
    {
        Destroy(gameObject, pieceDuration);
    }
}

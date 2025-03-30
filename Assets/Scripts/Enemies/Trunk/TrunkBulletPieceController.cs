using UnityEngine;

public class TrunkBulletPieceController : ProjectileController
{
    [SerializeField] private float pieceDuration = 3f;

    private void Start()
    {
        Destroy(gameObject, pieceDuration);
    }
}

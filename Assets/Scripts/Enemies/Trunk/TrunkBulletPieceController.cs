using UnityEngine;

public class TrunkBulletPieceController : MonoBehaviour
{
    [SerializeField] private float pieceDuration = 5f;

    private void Start()
    {
        Destroy(gameObject, pieceDuration);
    }
}

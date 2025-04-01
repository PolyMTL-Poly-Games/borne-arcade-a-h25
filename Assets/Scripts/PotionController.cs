using UnityEngine;

public class PotionController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponent<PlayerController>().FullHeal();
        Destroy(gameObject);
    }
}

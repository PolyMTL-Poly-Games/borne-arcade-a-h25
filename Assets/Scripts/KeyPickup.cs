using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(door);
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.isAtBoss = true;
        }
    }
}

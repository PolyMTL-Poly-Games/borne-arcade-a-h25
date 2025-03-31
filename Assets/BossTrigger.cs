using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public PlayerController player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.isAtBoss = true;
        }
    }
}

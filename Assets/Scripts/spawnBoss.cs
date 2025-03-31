using UnityEngine;

public class spawnBoss : MonoBehaviour
{
    public GameObject wall;
    public GameObject boss;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            wall.SetActive(true);
            boss.SetActive(true);
        }
    }
}

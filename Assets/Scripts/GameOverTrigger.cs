using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverTrigger : MonoBehaviour
{
    public GameObject gameOverUI;

    private void Start()
    {
        gameOverUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ReturnToMainMenu();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
        }
    }

    public void ReturnToMainMenu()
    {
        PlayerController.isAtBoss = false;
        Time.timeScale = 1f;
        AudioManagerController.instance?.Destroy();
        SceneManager.LoadScene("MainMenuScene");
    }
}

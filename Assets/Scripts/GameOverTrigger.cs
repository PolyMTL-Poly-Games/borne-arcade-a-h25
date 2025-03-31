using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // For PlayerInputActions

public class GameOverTrigger : MonoBehaviour
{
    public GameObject gameOverUI;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Menu.performed += _ => ReturnToMainMenu();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        gameOverUI.SetActive(false);
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

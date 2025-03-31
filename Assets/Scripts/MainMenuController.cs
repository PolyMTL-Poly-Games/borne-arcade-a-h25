using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // For PlayerInputActions

public class MainMenuController : MonoBehaviour
{
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Start.performed += _ => PlayGame();
        inputActions.Player.Exit.performed += _ => QuitGame();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

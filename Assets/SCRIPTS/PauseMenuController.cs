using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    private bool isPaused = false; // tracks whether the game is currently paused or not
    private PlayerInput playerInput; // reference to the Player's input component

    private void Awake()
    {
        // grab the Player's input component so we can disable it while paused
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // if paused, resume
            // if not paused, pause
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        // hide the pause menu and unfreeze the game
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false; // unpause all audio
        playerInput.ActivateInput(); // restore player input when resuming
        isPaused = false;
    }

    void Pause()
    {
        // show the pause menu and freeze the game
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true; // pause all audio
        playerInput.DeactivateInput(); // stop player from receiving any input while paused
        isPaused = true;
    }

    public void RestartGame()
    {
        // restart the game
        Time.timeScale = 1f;
        AudioListener.pause = false;
        AudioListener.volume = 0f;
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMainMenu()
    {
        // go back to main menu and reset time & audio before leaving the scene
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("StartMenuScene");
    }



}
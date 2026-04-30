using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    // for the GameOverPanel UI object
    public GameObject gameOverPanel;

    // how long to wait after death before showing the GameOver screen (this is to tune to match the death animation)
    public float delayBeforeGameOver = 1.2f;

    // game over sound
    public AudioClip gameOverSound;
    public float gameOverSoundVolume = 1f;

    private AudioSource audioSource; // AudioSource for the GameOver sound

    private void Awake()
    {
        // grab object's AudioSource for the game over sound
        audioSource = GetComponent<AudioSource>();

        // this makes the gameover sound play even while AudioListener is paused
        if (audioSource != null)
            audioSource.ignoreListenerPause = true;
    }

    // called by the Player's Damageable damageableDeath event when the knight dies
    public void ShowGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // wait in real time so the death animation fully plays before anything happens
        // WaitForSecondsRealtime ignores Time.timeScale so this always works correctly
        yield return new WaitForSecondsRealtime(delayBeforeGameOver);

        // cut the music immediately
        GameObject music = GameObject.Find("Music");
        if (music != null)
            music.GetComponent<AudioSource>().Stop();

        // freeze the game (stops all physics/movements and animator)
        Time.timeScale = 0f;

        // pause all audio (for example: this stops the Bringer walk loop and Flying Eye wing flap)
        AudioListener.pause = true;

        // play the GameOver sound if assigned (plays through ignoreListenerPause AudioSource)
        if (audioSource != null && gameOverSound != null)
            audioSource.PlayOneShot(gameOverSound, gameOverSoundVolume);

        // show the GameOver panel
        gameOverPanel.SetActive(true);
    }

    public void RetryGame()
    {
        // same as PauseMenuController.RestartGame (this resets everything and reloads fresh)
        Time.timeScale = 1f;
        AudioListener.pause = false;
        AudioListener.volume = 0f; // MusicManager.Start() restores this to 1f automatically
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMainMenu()
    {
        // reset time and audio then go back to the start menu
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("StartMenuScene");
    }
}
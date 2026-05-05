using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class VictoryController : MonoBehaviour
{
    // for the VictoryPanel object
    public GameObject victoryPanel;

    // for the Player object
    public PlayerInput playerInput;

    // how long to wait before showing the victory screen
    public float delayBeforeVictory = 0.5f;

    // victory sound
    public AudioClip victorySound;
    public float victorySoundVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        // grabs VictoryPanel AudioSource for the victory sound
        audioSource = GetComponent<AudioSource>();

        // victory sound plays even while AudioListener is paused
        if (audioSource != null)
            audioSource.ignoreListenerPause = true;
    }

    // called by the GoalZone script when the player touches it
    public void ShowVictory()
    {
        // IEnumerator means this function is allowed to pause mid-way through
        // without StartCoroutine, VictorySequence never actually runs
        StartCoroutine(VictorySequence());
    }
    private IEnumerator VictorySequence()
    {
        // pause here and wait in real time before doing anything
        // WaitForSecondsRealtime ignores Time.timeScale so the delay always works even if game is frozen
        yield return new WaitForSecondsRealtime(delayBeforeVictory);

        // cut all music
        GameObject music = GameObject.Find("Music");
        if (music != null)
            music.GetComponent<AudioSource>().Stop();

        // freeze everything (enemies stop moving and animators stop)
        Time.timeScale = 0f;

        // all active audios are pause (player and enemy SFX and loops)
        AudioListener.pause = true;

        // stops all player input (movement, attacks, everything)
        if (playerInput != null)
            playerInput.DeactivateInput();

        // ignoreListenerPause lets this play even though AudioListener is paused
        if (audioSource != null && victorySound != null)
            audioSource.PlayOneShot(victorySound, victorySoundVolume);

        // show the victory panel
        victoryPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        // reset everything and reload fresh
        Time.timeScale = 1f;
        AudioListener.pause = false;
        AudioListener.volume = 0f;
        // re-enable input before reload just in case
        if (playerInput != null)
            playerInput.ActivateInput();
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
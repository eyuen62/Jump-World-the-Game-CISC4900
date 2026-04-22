using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        // load the main game scene when the player clicks Start
        SceneManager.LoadScene("GameScene");
    }

    public void OnQuitClick()
    {
        // stop play mode if running in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // quit the application if running as a standalone build
#elif UNITY_STANDALONE
        Application.Quit();
        // load QuitScene if running as WebGL since WebGL cant truly quit
#elif UNITY_WEBGL
        SceneManager.LoadScene("QuitScene");
#endif
    }
}
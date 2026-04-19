using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // the red text prefab that spawns when a character takes damage
    public GameObject damageTextPrefab;

    // the green text prefab that spawns when a character gets healed
    public GameObject healthTextPrefab;

    // reference to the Canvas in the scene
    public Canvas gameCanvas;

    private void Awake()
    {
        // automatically find the Canvas in the scene
        gameCanvas = FindFirstObjectByType<Canvas>();
    }

    private void OnEnable()
    {
        // subscribe to CharacterEvents so this script starts listening for damage and heal events
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
    }

    private void OnDisable()
    {
        // unsubscribe from CharacterEvents when this object is disabled so we stop listening
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
    }

    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        // convert the character's world position to a screen position so the text appears in the right spot
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position) + new Vector3(0, 50, 0);

        // spawn a copy of the damage text prefab at the screen position
        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString(); // set the text to show the actual damage number
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        // same as CharacterTookDamage but uses the green health text prefab and shows the heal amount instead
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position) + new Vector3(0, 50, 0);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString(); // set the text to show the actual heal number
    }

    public void OnExitGame(InputAction.CallbackContext context) // runs when the Escape key is pressed
    {
        if (context.started) // only trigger once when the key is first pressed
        {
            // log a message to confirm OnExitGame is firing in the console
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);

#if UNITY_EDITOR
            // if running in the Unity Editor, stop play mode instead of quitting
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            // if running as a standalone build (.exe), quit the application
            Application.Quit();
#elif UNITY_WEBGL
            // if running as a WebGL build, load the QuitScene since WebGL cant truly quit
            SceneManager.LoadScene("QuitScene");
#endif
        }
    }
}
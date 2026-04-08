using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab; // the red text prefab that spawns when a character takes damage
    public GameObject healthTextPrefab; // the green text prefab that spawns when a character gets healed

    public Canvas gameCanvas; // reference to the Canvas in the scene

    private void Awake()
    {
        // automatically find the Canvas in the scene so we don't have to drag it in manually
        gameCanvas = FindFirstObjectByType<Canvas>();
    }

    private void OnEnable()
    {
        // subscribe to the CharacterEvents so this UIManager script starts listening for damage and heal events
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
        // convert the character's world position to a screen position so the text appears in the right spot on screen
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position) + new Vector3(0, 50, 0);

        // spawn a copy of the damage text prefab at the screen position
        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();

        // set the text to show the actual damage number as a string
        tmpText.text = damageReceived.ToString();
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        // same as CharacterTookDamage but uses the health text prefab (green) and shows the heal amount instead
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position) + new Vector3(0, 50, 0);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();

        // set the text to show the actual heal number as a string
        tmpText.text = healthRestored.ToString();
    }

    public void OnExitGame(InputAction.CallbackContext context) // called when the Escape key is pressed
    {
        if (context.started) // only trigger once when the key is first pressed
        {
            // log a message so we can confirm OnExitGame is firing in the console
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);

#if UNITY_EDITOR
            // if running in the Unity Editor, stop play mode instead of quitting
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            // if running as a standalone build (.exe), quit the application
            Application.Quit();
#elif UNITY_WEBGL
            // if running as a WebGL build, load the quit scene since WebGL cant truly quit
            SceneManager.LoadScene("QuitScene");
#endif
        }
    }
}
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab; // the red text prefab that spawns when a character takes damage
    public GameObject healthTextPrefab; // the green text prefab that spawns when a character gets healed

    public Canvas gameCanvas; // reference to the Canvas in the scene

    private void Awake()
    {
        // automatically find the Canvas in the scene so we don't have to drag it in manually
        gameCanvas = FindObjectOfType<Canvas>();
    }

    private void OnEnable()
    {
        // subscribe to the CharacterEvents so this UIManager script starts listening for damage and heal events
        // whenever a character takes damage or gets healed, the matching method below will run automatically
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
    }

    private void OnDisable()
    {
        // unsubscribe from CharacterEvents when this object is disabled so we stop listening
        // this prevents leftover listeners from firing after this object is gone
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
    }

    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        // convert the character's world position to a screen position so the text appears in the right spot on screen
        // + new Vector3(0, 50, 0) pushes the text slightly upward from the character's pivot point
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position) + new Vector3(0, 50, 0);

        // spawn a copy of the damage text prefab at the screen position then immediately grab its text mesh pro component so we can set the text value
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
}
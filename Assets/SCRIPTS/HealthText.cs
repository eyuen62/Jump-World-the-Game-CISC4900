using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    // how fast and which direction the text moves (X is left/right, Y is up/down, Z is toward/away from screen)
    // set Y to 75 so it floats upward at 75 pixels per second
    public Vector3 moveSpeed = new Vector3(0, 75, 0);

    // how long in seconds before the text fully disappears
    public float timeToFade = 1f;

    RectTransform textTransform; // reference to the RectTransform so we can move the text around on screen
    TextMeshProUGUI textMeshPro; // reference to the TextMeshPro component

    private float timeElapsed = 0f; // tracks how much time has passed since the text appeared
    private Color startColor; // stores the original color before the fade begins

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>(); // get the RectTransform component
        textMeshPro = GetComponent<TextMeshProUGUI>(); // get the TextMeshPro component
        startColor = textMeshPro.color; // store the starting color so we know what to fade from
    }

    private void Update()
    {
        // float the text upward every frame based on moveSpeed
        textTransform.position += moveSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime; // count up how much time has passed

        if (timeElapsed < timeToFade) // if the fade time hasnt been reached yet
        {
            // calculate how transparent the text should be
            // starts fully visible and gets more transparent the closer it gets to timeToFade
            float fadeAlpha = startColor.a * (1 - (timeElapsed / timeToFade));

            // apply the faded color while keeping the original red or green color unchanged
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);
        }
        else
        {
            Destroy(gameObject); // text has fully faded so it gets removed from the scene
        }
    }
}
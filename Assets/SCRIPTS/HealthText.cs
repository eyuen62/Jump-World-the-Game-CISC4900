using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    // how fast and which direction the text moves - Y being 75 means it floats upward at 75 pixels per second
    public Vector3 moveSpeed = new Vector3(0, 75, 0);

    // how long in seconds before the text fully disappears
    public float timeToFade = 1f;

    RectTransform textTransform; // reference to the RectTransform so we can move the text around on screen
    TextMeshProUGUI textMeshPro; // reference to the TextMeshPro component

    private float timeElapsed = 0f; // keeps track of how much time has passed since the text appeared
    private Color startColor; // stores the original color of the text so we can fade from it

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>(); // find and store the RectTransform on this object
        textMeshPro = GetComponent<TextMeshProUGUI>(); // find and store the TextMeshPro component on this object
        startColor = textMeshPro.color; // save the starting color (red or green) so we know what to fade from
    }

    private void Update()
    {
        // move the text upward every frame based on moveSpeed - the faster moveSpeed is = the quicker it floats up
        textTransform.position += moveSpeed * Time.deltaTime;

        // add up how much time has passed since this text was spawned
        timeElapsed += Time.deltaTime;

        if (timeElapsed < timeToFade) // if we haven't hit the fade time limit yet, text keeps fading
        {
            // calculate how transparent the text should be
            // starts fully visible and gets more transparent the closer we get to timeToFade
            float fadeAlpha = startColor.a * (1 - (timeElapsed / timeToFade));

            // apply the new faded color while keeping the original red or green color the same
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);
        }
        else
        {
            // if time is up - the text has fully faded so it destroy the object and remove it from the scene
            Destroy(gameObject);
        }
    }
}
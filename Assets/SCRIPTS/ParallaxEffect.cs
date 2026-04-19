using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // the camera that's looking at the game
    public Camera cam;

    // how fast the background moves relative to the camera (0 = moves with camera, 1 = stays still)
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.5f; // set as 0.5 for inbetween

    private Vector2 startingPosition; // stores where the background started when the game began
    private Vector2 lastCameraPosition; // stores where the camera was last frame

    void Start()
    {
        startingPosition = transform.position; // save the starting position of the background
        lastCameraPosition = cam.transform.position; // save the starting position of the camera
    }

    void LateUpdate() // runs after the camera moves every frame
    {
        // calculate how much the camera moved this frame
        Vector2 cameraMovement = (Vector2)cam.transform.position - lastCameraPosition;

        // move the background slower than the camera to create the parallax depth effect
        transform.position += new Vector3(cameraMovement.x * parallaxSpeed, 0, 0);

        // remember where the camera is now for the next frame
        lastCameraPosition = cam.transform.position;
    }
}
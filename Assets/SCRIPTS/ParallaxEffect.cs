using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // camera that's looking at the game
    public Camera cam;


    // slider range from 0 to 1 for the parallax speed
    // if i want it to move with the camera, then i want it at 1
    // if i want it to move with the camera, then i want it at 0
    [Range(0f, 1f)]

    // start at 0.5
    public float parallaxSpeed = 0.5f;

    // remember where the background started when the game began
    private Vector2 startingPosition;

    // remember where the camera started
    private Vector2 lastCameraPosition;

    void Start()
    {
        // save where we started
        startingPosition = transform.position;
        lastCameraPosition = cam.transform.position;
    }

    void LateUpdate() // happens AFTER the camera moves
    {
        // this is to show how much did the camera move
        Vector2 cameraMovement = (Vector2)cam.transform.position - lastCameraPosition;

        // move the background but slower than the camera
        transform.position += new Vector3(cameraMovement.x * parallaxSpeed, 0, 0);

        // remember where the camera is now for next frame
        lastCameraPosition = cam.transform.position;
    }
}
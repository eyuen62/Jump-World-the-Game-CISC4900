using UnityEngine;

public class PlayerRunSoundController : MonoBehaviour
{
    public AudioClip runSound; // drag dirtchainrun4 here in the Inspector
    public AudioClip crouchWalkSound; // drag dirtchainwalk1 here in the Inspector
    public float runVolume = 1f; // volume for the run sound
    public float crouchWalkVolume = 0.8f; // volume for the crouch walk sound — slightly quieter since crouching is sneaky

    public float stopDelay = 0.1f; // how long to wait before actually stopping the sound — prevents restart clicks on fast A/D switches

    private AudioSource audioSource; // reference to the AudioSource component on the Player
    private Animator animator; // reference to the Animator component on the Player
    private float stopTimer = 0f; // counts up when the Player stops moving — sound only stops after this exceeds stopDelay

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // find and store the AudioSource on the Player
        animator = GetComponent<Animator>(); // find and store the Animator on the Player
    }

    private void Update()
    {
        // read the current state of the Player from the Animator every frame
        bool isMoving = animator.GetBool("isMoving"); // is the Player moving horizontally
        bool isGrounded = animator.GetBool("isGrounded"); // is the Player on the ground
        bool isCrouching = animator.GetBool("isCrouching"); // is the Player crouching

        if (isGrounded && isMoving && !isCrouching) // Player is running normally on the ground
        {
            stopTimer = 0f; // reset the stop timer

            if (!audioSource.isPlaying || audioSource.clip != runSound) // only swap or start if not already playing run sound
            {
                audioSource.clip = runSound; // assign the run clip
                audioSource.volume = runVolume; // set the run volume
                audioSource.loop = true; // keep looping while the Player is running
                audioSource.Play(); // start playing
            }
        }
        else if (isGrounded && isMoving && isCrouching) // Player is crouch walking
        {
            stopTimer = 0f; // reset the stop timer

            if (!audioSource.isPlaying || audioSource.clip != crouchWalkSound) // only swap or start if not already playing crouch walk sound
            {
                audioSource.clip = crouchWalkSound; // assign the crouch walk clip
                audioSource.volume = crouchWalkVolume; // set the crouch walk volume
                audioSource.loop = true; // keep looping while the Player is crouch walking
                audioSource.Play(); // start playing
            }
        }
        else // Player is idle, jumping, attacking, or dead
        {
            if (audioSource.isPlaying) // only bother with the stop timer if something is actually playing
            {
                stopTimer += Time.deltaTime; // count up how long the Player has been not moving

                if (stopTimer >= stopDelay) // only stop after the delay window has passed
                {
                    audioSource.Stop(); // cut the sound
                    stopTimer = 0f; // reset the timer
                }
            }
        }
    }
}
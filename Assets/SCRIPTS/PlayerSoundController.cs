using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    // run and crouch walk sounds
    public AudioClip runSound; // drag a run sfx here
    public AudioClip crouchWalkSound; // drag a crouch walk sfx here
    public float runVolume = 1f; // volume for the run sound
    public float crouchWalkVolume = 0.8f; // volume for the crouch walk sound

    // jump and land sounds
    public AudioClip jumpSound; // drag jump sfx here
    public AudioClip landSound; // drag land sfx here
    public float jumpVolume = 1f; // volume for the jump sound
    public float landVolume = 1f; // volume for the land sound

    // timing
    public float stopDelay = 0.1f; // how long to wait before stopping loop sounds (prevents restarting clicks on fast A/D key switches)

    private AudioSource loopAudioSource; // AudioSource 1 (handles looping sounds like run and crouch walk)
    private AudioSource oneshotAudioSource; // AudioSource 2 (handles one shot sounds like jump and land)

    private Animator animator; // reference to the Animator component on the Player

    private float stopTimer = 0f; // counts up when the Player stops moving (loop sound only stops after this exceeds stopDelay)
    private bool wasGrounded = false; // tracks if the Player was on the ground last frame (used to detect the exact moment of landing)

    private void Awake()
    {
        // grab both AudioSource 1 & 2 components on the Player
        AudioSource[] sources = GetComponents<AudioSource>();
        loopAudioSource = sources[0]; // first AudioSource (looping sounds)
        oneshotAudioSource = sources[1]; // second AudioSource (one shot sounds)

        animator = GetComponent<Animator>(); // get the Animator component
    }

    private void Update()
    {
        // read the current state of the Player from the Animator every frame
        bool isMoving = animator.GetBool("isMoving"); // is the Player moving horizontally (left/right)
        bool isGrounded = animator.GetBool("isGrounded"); // is the Player on the ground
        bool isCrouching = animator.GetBool("isCrouching"); // is the Player crouching
        bool isAlive = animator.GetBool("isAlive"); // is the Player still alive

        HandleLoopSounds(isMoving, isGrounded, isCrouching, isAlive); // handle run and crouch walk looping
        HandleJumpSound(isGrounded, isAlive); // handle jump and land one shots
    }

    private void HandleLoopSounds(bool isMoving, bool isGrounded, bool isCrouching, bool isAlive)
    {
        // check if the Player is currently in any attack state (if so, stop all loop sounds immediately)
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = stateInfo.IsName("PlayerAttackNoMovement") ||
                           stateInfo.IsName("PlayerAttack2YesMovement") ||
                           stateInfo.IsName("PlayerCrouchAttackAnimation");

        if (isAttacking) // stop all loop sounds during any attack (no run or crouch walk should play while attacking)
        {
            if (loopAudioSource.isPlaying)
            {
                loopAudioSource.Stop(); // cut the loop sound immediately
                stopTimer = 0f; // reset the timer
            }
            return; // skip the rest (no loop sounds while attacking)
        }

        if (isGrounded && isMoving && !isCrouching && isAlive) // Player is only running on the ground
        {
            stopTimer = 0f; // reset the stop timer

            if (!loopAudioSource.isPlaying || loopAudioSource.clip != runSound) // only swap or start if not already playing the run sound
            {
                loopAudioSource.clip = runSound; // assign the run sfx
                loopAudioSource.volume = runVolume; // set the run volume
                loopAudioSource.loop = true; // keep looping while running
                loopAudioSource.Play(); // start playing
            }
        }
        else if (isGrounded && isMoving && isCrouching && isAlive) // Player is crouch walking
        {
            stopTimer = 0f; // reset the stop timer

            if (!loopAudioSource.isPlaying || loopAudioSource.clip != crouchWalkSound) // only swap or start if not already playing the crouch walk sound
            {
                loopAudioSource.clip = crouchWalkSound; // assign the crouch walk sfx
                loopAudioSource.volume = crouchWalkVolume; // set the crouch walk volume
                loopAudioSource.loop = true; // keep looping while crouch walking
                loopAudioSource.Play(); // start playing
            }
        }
        else // Player is idle, jumping, or dead
        {
            if (loopAudioSource.isPlaying) // only bother with the stop timer if something is actually playing
            {
                stopTimer += Time.deltaTime; // count up how long the Player has been not moving

                if (stopTimer >= stopDelay) // only stop after the delay window has passed
                {
                    loopAudioSource.Stop(); // cut the sound
                    stopTimer = 0f; // reset the timer
                }
            }
        }
    }

    private void HandleJumpSound(bool isGrounded, bool isAlive)
    {
        if (!isAlive) return; // dont play any sounds if the Player is dead

        if (!isGrounded && wasGrounded) // Player just left the ground (this is the jump moment)
        {
            oneshotAudioSource.PlayOneShot(jumpSound, jumpVolume); // play the jump sound once
        }

        if (isGrounded && !wasGrounded) // Player just touched the ground (this is the land moment)
        {
            oneshotAudioSource.PlayOneShot(landSound, landVolume); // play the land sound once
        }

        wasGrounded = isGrounded; // store this frame's grounded state for next frame comparison
    }
}
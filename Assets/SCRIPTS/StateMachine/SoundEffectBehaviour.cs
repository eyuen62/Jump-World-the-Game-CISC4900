using UnityEngine;

public class SoundEffectBehaviour : StateMachineBehaviour
{
    // the sfx to play when this state is entered
    public AudioClip soundToPlay;

    // how loud the sfx is (1 is full volume)
    public float volume = 1f;

    // if checked, plays the sfx immediately when the state is entered
    public bool playOnEnter = true;

    // if checked, plays the sfx when the state is exited
    public bool playOnExit = false;

    // if checked, waits for playDelay seconds before playing the sfx
    public bool playAfterDelay = false;

    // how long to wait before playing the sfx (only used if playAfterDelay is checked)
    public float playDelay = 0.25f;

    private float timeSinceEntered = 0; // tracks how much time has passed since entering the state
    private bool hasDelayedSoundPlayed = false; // prevents the delayed sfx from playing more than once per state entry

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnEnter) // play immediately on entry if checked
        {
            // grab all AudioSources on the character (use the second one if it exists, otherwise fall back to the first one)
            AudioSource[] sources = animator.gameObject.GetComponents<AudioSource>();
            AudioSource audioSource = sources.Length >= 2 ? sources[1] : sources[0];
            if (audioSource != null) // only play if an AudioSource exists on the character
            {
                audioSource.PlayOneShot(soundToPlay, volume); // play the sfx once without interrupting any other sounds
            }
        }

        // reset the delay timer and flag every time this state is entered
        timeSinceEntered = 0;
        hasDelayedSoundPlayed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playAfterDelay && !hasDelayedSoundPlayed) // only run if delay mode is on and sfx hasnt played yet
        {
            timeSinceEntered += Time.deltaTime; // count up time since entering the state

            if (timeSinceEntered > playDelay) // once the delay time has passed, play the sfx
            {
                // same as OnStateEnter (grab the correct AudioSource and play through it)
                AudioSource[] sources = animator.gameObject.GetComponents<AudioSource>();
                AudioSource audioSource = sources.Length >= 2 ? sources[1] : sources[0];
                if (audioSource != null) // only play if an AudioSource exists on the character
                {
                    audioSource.PlayOneShot(soundToPlay, volume); // play the delayed sfx once
                }
                hasDelayedSoundPlayed = true; // mark that the sfx has played so it doesnt repeat
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit) // play on exit if checked
        {
            // same as OnStateEnter (grab the correct AudioSource and play through it)
            AudioSource[] sources = animator.gameObject.GetComponents<AudioSource>();
            AudioSource audioSource = sources.Length >= 2 ? sources[1] : sources[0];
            if (audioSource != null) // only play if an AudioSource exists on the character
            {
                audioSource.PlayOneShot(soundToPlay, volume); // play the exit sfx once
            }
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
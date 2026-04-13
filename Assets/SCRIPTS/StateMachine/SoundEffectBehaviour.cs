using UnityEngine;

public class SoundEffectBehaviour : StateMachineBehaviour
{
    public AudioClip soundToPlay; // the sound effect to play when this state is entered
    public float volume = 1f; // how loud the sound effect is - 1 is full volume, lower is quieter
    public bool playOnEnter = true; // if checked, plays the sound immediately when the state is entered
    public bool playOnExit = false; // if checked, plays the sound when the state is exited
    public bool playAfterDelay = false; // if checked, waits for playDelay seconds before playing

    public float playDelay = 0.25f; // how long to wait before playing the sound if playAfterDelay is checked

    private float timeSinceEntered = 0; // tracks how much time has passed since entering the state
    private bool hasDelayedSoundPlayed = false; // prevents the delayed sound from playing more than once per state entry

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnEnter) // play immediately on entry if checked
        {
            AudioSource.PlayClipAtPoint(soundToPlay, Camera.main.transform.position, volume);
        }

        // reset the delay timer and flag every time we enter this state
        timeSinceEntered = 0;
        hasDelayedSoundPlayed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playAfterDelay && !hasDelayedSoundPlayed) // only run if delay mode is on and sound hasnt played yet
        {
            timeSinceEntered += Time.deltaTime; // count up time since entering the state

            if (timeSinceEntered > playDelay) // once delay time is passed, play the sound
            {
                AudioSource.PlayClipAtPoint(soundToPlay, Camera.main.transform.position, volume);
                hasDelayedSoundPlayed = true; // mark that the sound has played so it doesnt repeat
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit) // play on exit if checked
        {
            AudioSource.PlayClipAtPoint(soundToPlay, Camera.main.transform.position, volume);
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
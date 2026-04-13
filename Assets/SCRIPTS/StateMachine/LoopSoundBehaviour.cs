using UnityEngine;

public class LoopSoundBehaviour : StateMachineBehaviour
{
    public AudioClip soundToPlay; // the audio clip that will loop while this state is active
    public float volume = 1f; // how loud the sound plays — 1 is full volume

    private AudioSource audioSource; // reference to the AudioSource component on the character

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // grab the AudioSource component sitting on the character — this is what actually plays the sound
        audioSource = animator.gameObject.GetComponent<AudioSource>();

        if (audioSource != null && soundToPlay != null) // only continue if both the AudioSource and clip actually exist
        {
            // only restart the clip if its not already playing — prevents double sound on quick A/D direction switches
            if (!audioSource.isPlaying || audioSource.clip != soundToPlay)
            {
                audioSource.clip = soundToPlay;
                audioSource.volume = volume;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioSource != null) // only stop if the AudioSource reference still exists
        {
            audioSource.Stop(); // stop the looping sound the moment this state is left
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
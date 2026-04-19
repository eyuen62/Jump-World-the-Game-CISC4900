using UnityEngine;

public class LoopSoundBehaviour : StateMachineBehaviour
{
    // the sfx that will loop while this state is active
    public AudioClip soundToPlay;

    // how loud the sound plays (1 is full volume)
    public float volume = 1f;

    // check this for enemies that need distance based volume (like the Flying Eye wing flap)
    // leave unchecked for flat 2D audio (like the Bringer walk)
    public bool use3DAudio = false;

    // how close it needs to be before the sound plays at full volume (only used if use3DAudio is checked)
    public float minDistance = 3f;

    // how far it can be before the sound becomes completely silent (only used if use3DAudio is checked)
    public float maxDistance = 6f;

    private AudioSource audioSource; // reference to the AudioSource component on the character

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // grab the AudioSource component on the character
        audioSource = animator.gameObject.GetComponent<AudioSource>();

        if (audioSource != null && soundToPlay != null) // only continue if both the AudioSource and sfx exist
        {
            audioSource.clip = soundToPlay; // assign the sfx
            audioSource.volume = volume; // set the volume
            audioSource.loop = true; // keep looping until the state exits

            if (use3DAudio) // only apply 3D spatial settings if this is enabled
            {
                audioSource.spatialBlend = 1f; // enable 3D audio so the sound fades with distance
                audioSource.minDistance = minDistance; // full volume within this range
                audioSource.maxDistance = maxDistance; // silent beyond this range
            }
            else
            {
                audioSource.spatialBlend = 0f; // keep fully 2D (flat volume with no distance falloff)
            }

            audioSource.Play(); // start playing the sound
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
        if (audioSource != null) // only stop if the AudioSource still exists
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
using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    public float fadeTime = 0.5f; // how long the fade out takes before the enemy is fully removed

    private float timeElapsed = 0f; // tracks how much time has passed since the fading started
    SpriteRenderer spriteRenderer; // reference to the SpriteRenderer so we can change the transparency
    GameObject objToRemove; // reference to the enemy so it gets destroyed after fading
    Color startColor; // stores the original color before the fade begins


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f; // reset the timer every time the death state is entered

        spriteRenderer = animator.GetComponent<SpriteRenderer>(); // get the SpriteRenderer from the enemy
        startColor = spriteRenderer.color; // store the original color so we know what to fade from
        objToRemove = animator.gameObject; // store the enemy so it gets destroyed after fading
    }

    // OnStateUpdate is called every frame while this state is active
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed += Time.deltaTime; // count up how much time has passed

        // calculate the new transparency (starts fully visible and fades to invisible over fadeTime seconds)
        float newAlpha = startColor.a * (1 - (timeElapsed / fadeTime));

        // apply the new transparency while keeping the original RGB color values unchanged
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

        if (timeElapsed > fadeTime) // once the fade is complete
        {
            Destroy(objToRemove); // destroy the enemy and remove it from the Hierarchy
        }
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    public float fadeTime = 0.5f; // how long the fade out takes in seconds
    private float timeElapsed = 0f; // tracks how much time has passed since the fade started

    SpriteRenderer spriteRenderer; // reference to the SpriteRenderer so we can change the alpha
    GameObject objToRemove; // reference to the enemy GameObject so we can destroy it
    Color startColor; // stores the original color so we can fade from it

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f; // reset the timer every time the death state is entered

        spriteRenderer = animator.GetComponent<SpriteRenderer>(); // get the SpriteRenderer from the enemy
        startColor = spriteRenderer.color; // store the original color (including original alpha)
        objToRemove = animator.gameObject; // store the enemy GameObject so we can destroy it later
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed += Time.deltaTime; // count up how much time has passed

        // calculate the new alpha — starts at full opacity and fades to 0 over fadeTime seconds
        float newAlpha = startColor.a * (1 - (timeElapsed / fadeTime));

        // apply the new color with the faded alpha while keeping the original RGB values
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

        // once the fade time is up, destroy the enemy GameObject and remove it from the Hierarchy
        if (timeElapsed > fadeTime)
        {
            Destroy(objToRemove);
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
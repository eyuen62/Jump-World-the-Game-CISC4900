using UnityEngine;

public class SetBoolBehaviour : StateMachineBehaviour
{
    // the name of the Bool parameter in the Animator we want to change
    public string boolName;

    // if checked, this runs when entering or exiting a plain state (grey box in the Animator)
    public bool updateOnState;

    // if checked, this runs when entering or exiting a sub-state machine (orange box in the Animator)
    public bool updateOnStateMachine;

    // the value to set when entering this state
    public bool valueOnEnter;

    // the value to set when exiting this state
    public bool valueOnExit;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnState)
        {
            animator.SetBool(boolName, valueOnEnter); // set the Bool to valueOnEnter when entering
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
        if (updateOnState)
        {
            animator.SetBool(boolName, valueOnExit); // set the Bool to valueOnExit when exiting
        }
    }

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

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine) // only run if updateOnStateMachine is checked
        {
            animator.SetBool(boolName, valueOnEnter); // set the Bool to valueOnEnter when entering
        }
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine) // only run if updateOnStateMachine is checked
        {
            animator.SetBool(boolName, valueOnExit); // set the Bool to valueOnExit when exiting
        }
    }
}
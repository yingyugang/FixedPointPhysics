using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// Controls the transition between dive animations in an Animator component, 
    /// setting the animator's state to continue the diving animation sequence.
    /// </summary>
    public class DiveToDiveContinue : StateMachineBehaviour
    {
        /// A static reference to the animator parameter 'State' to avoid recalculating the hash in each instance.
        private static readonly int State = Animator.StringToHash("State");

        /// <summary>
        /// Called when a transition starts and the state machine starts to evaluate this state.
        /// Sets the animator's 'State' parameter to indicate the continuation of a dive animation.
        /// </summary>
        /// <param name="animator">The Animator component the state machine is attached to.</param>
        /// <param name="stateInfo">Contains information about the current state.</param>
        /// <param name="layerIndex">The layer index of the state.</param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Update the animator's state to continue the dive animation.
            animator.SetInteger(State, ActorStateConstant.ANIM_DIVE_CONTINUE);
        }
    }
}
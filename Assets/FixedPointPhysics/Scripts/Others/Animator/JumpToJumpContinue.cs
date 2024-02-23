using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// Manages the transition from a jump state to a falling state in an Animator component,
    /// updating the animator's state to reflect the change.
    /// </summary>
    public class JumpToJumpContinue : StateMachineBehaviour
    {
        /// A static reference to the animator parameter 'State', optimizing performance by avoiding hash recalculations.
        private static readonly int State = Animator.StringToHash("State");

        /// <summary>
        /// Called when transitioning into the state this script is attached to. It updates the animator's 'State'
        /// parameter to indicate the actor is in the falling phase of a jump.
        /// </summary>
        /// <param name="animator">The Animator component this state machine behavior is attached to.</param>
        /// <param name="stateInfo">Information about the current state of the animator.</param>
        /// <param name="layerIndex">The layer index within the Animator where this state is located.</param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Set the Animator's state to indicate the actor is now falling.
            animator.SetInteger(State, ActorStateConstant.ANIM_FALLING);
        }
    }
}
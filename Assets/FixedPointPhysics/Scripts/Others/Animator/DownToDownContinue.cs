using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// Manages the transition to continue the "down" animation in an Animator component,
    /// setting the animator's state appropriately upon entering the associated state.
    /// </summary>
    public class DownToDownContinue : StateMachineBehaviour
    {
        /// A static reference to the animator parameter 'State' to optimize performance by avoiding recalculations of the hash.
        private static readonly int State = Animator.StringToHash("State");

        /// <summary>
        /// Called automatically by Unity when transitioning into a state. It sets the animator's 'State'
        /// parameter to the constant value indicating the continuation of a "down" animation sequence.
        /// </summary>
        /// <param name="animator">The Animator component the state machine behavior is attached to.</param>
        /// <param name="stateInfo">Information about the current animator state.</param>
        /// <param name="layerIndex">The index of the layer where the state resides.</param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(State, ActorStateConstant.ANIM_DOWN_CONTINUE);
        }
    }
}
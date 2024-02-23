using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// An abstract base class for game objects that use fixed-point mathematics for physics simulations,
    /// ensuring deterministic behavior across different platforms. It requires a FixedPointTransform component
    /// and is designed to work within a custom physics engine that uses fixed-point calculations.
    /// </summary>
    [RequireComponent(typeof(FPTransform))] // Ensures that any inheritor of this class must also have a FixedPointTransform component.
    [DefaultExecutionOrder(-898)] // Specifies the execution order of this script to ensure it runs before most other scripts.
    public abstract class FPGameObject : MonoBehaviour, FastListItem
    {
        // Provides access to the FixedPointTransform component associated with this game object.
        public FPTransform fpTransform { get; private set; }

        // Abstract method that should be overridden to define how this object should update its view or rendering.
        public abstract void OnViewUpdate();

        // Abstract method that should be overridden to define the logic update for this object, typically called each physics step.
        public abstract void OnLogicUpdate();

        // Implements the index property from FastListItem, used to track this object's position in a FastList collection.
        public int index { get; set; }

        // A collection of FixedPointTimer objects, allowing this game object to manage multiple timers efficiently.
        internal readonly FastList<FPTimer> fixedPointTimers = new ();

        private void Awake()
        {
            // On Awake, add this object to the global list of fixed point game objects if the application is playing.
            if (Application.isPlaying)
            {
                FPPhysicsPresenter.Instance.fixedPointGameObjectFastList.Add(this);
            }
            // Initialize the FixedPointTransform component.
            fpTransform = GetComponent<FPTransform>();
            // Call the Init method, which can be overridden by subclasses to perform additional initialization.
            Init();
        }

        /// <summary>
        /// A protected virtual method that can be overridden by subclasses to perform initialization tasks.
        /// This method is called during the Awake phase.
        /// </summary>
        protected virtual void Init(){}
    }
}
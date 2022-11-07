using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public abstract class FixedPointGameObject : MonoBehaviour
    {
        public FixedPointTransform fixedPointTransform { get; protected set; }
        protected bool isDirty { get; set; }
        protected abstract void OnTransfered();
        public abstract void OnViewUpdate();
        public abstract void DrawGizmosSelected();
        public abstract void OnLogicUpdate();
        public int indexInActorList { get;  set; }
    }
}
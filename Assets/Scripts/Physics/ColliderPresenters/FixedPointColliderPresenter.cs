using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointColliderPresenter : MonoBehaviour
    {
        public ColliderType colliderType { get; protected set; }
        public FixedPointCollider fixedPointCollider { get; protected set; }

        [SerializeField]
        protected bool isTrigger;
        [SerializeField]
        protected int layer;
        private void OnDisable()
        {
            if(fixedPointCollider != null)
                fixedPointCollider.enabled = false;
        }

        private void OnEnable()
        {
            if (fixedPointCollider != null)
                fixedPointCollider.enabled = true;
        }
    }
}
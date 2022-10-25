using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class TestManifoldAndImpulse : MonoBehaviour
    {
        public FixedPointSphereColliderPresenter sphere1;
        public Rigidbody sphere2;

        private void FixedUpdate()
        {
            FixedPointPhysicsPresenter.Instance.OnUpdate();
        }
    }
}
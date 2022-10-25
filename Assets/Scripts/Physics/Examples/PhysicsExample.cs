using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsExample : MonoBehaviour
    {
        private void FixedUpdate()
        {
            FixedPointPhysicsPresenter.Instance.OnUpdate();
        }
    }
}
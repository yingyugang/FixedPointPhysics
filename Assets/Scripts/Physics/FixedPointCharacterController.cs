using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointCharacterController: MonoBehaviour
    {
        public FixedPointVector3 impulse;
        public FixedPointTransform fixedPointTransform;

        private void Awake()
        {
            fixedPointTransform = new FixedPointTransform(null, name);
            fixedPointTransform.fixedPointPosition = new FixedPointVector3(transform.position);
        }

        public void OnUpdate()
        {

        }
    }
}
using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointCharacterController: MonoBehaviour
    {
        [HideInInspector]
        public FixedPointVector3 impulse;
        [HideInInspector]
        public FixedPointTransform fixedPointTransform;
        public FixedPoint64 radius = 0.5;
        public FixedPoint64 mass = 1;
        FixedPointVector3 velocity;
        private void Awake()
        {
            fixedPointTransform = new FixedPointTransform(null, name);
            fixedPointTransform.fixedPointPosition = new FixedPointVector3(transform.position);
            FixedPointPhysicsPresenter.Instance.actors.Add(this);
        }

        public void Move(FixedPointVector3 velocity)
        {
            this.velocity = velocity;
        }

        public void AddImpulse(FixedPointVector3 additionalImpulse)
        {
            if (impulse == FixedPointVector3.zero)
            {
                impulse = additionalImpulse;
            }
            else
            {
                var impulseNormal = impulse.normalized;
                var magnitude = impulse.magnitude;
                var dot = FixedPointVector3.Dot(additionalImpulse, impulseNormal);
                if (dot > magnitude)
                {
                    impulse = additionalImpulse;
                }
                else if (dot > 0)
                {
                    impulse = impulse + additionalImpulse - impulseNormal * dot;
                }
                else
                {
                    impulse = impulse + additionalImpulse;
                }
            }
        }

        public void OnUpdate()
        {
            var finalOrientation = velocity + impulse;
            impulse = FixedPointVector3.zero;
            fixedPointTransform.fixedPointPosition += finalOrientation;
            transform.position = fixedPointTransform.fixedPointPosition.ToVector3();
        }
    }
}
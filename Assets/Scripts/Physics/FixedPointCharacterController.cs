using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointCharacterController: MonoBehaviour
    {
        [HideInInspector]
        public FixedPointVector3 impulse;
        [HideInInspector]
        public FixedPointVector3 constraint;
        [HideInInspector]
        public FixedPointTransform fixedPointTransform;
        [HideInInspector]
        public FixedPoint64 radius = 0.5;
        [HideInInspector]
        public FixedPoint64 mass = 1;
      
        FixedPointVector3 velocity;

        FixedPointVector3 forces;

        private void Awake()
        {
            fixedPointTransform = new FixedPointTransform(null, name);
            fixedPointTransform.fixedPointPosition = new FixedPointVector3(transform.position);
            FixedPointPhysicsPresenter.Instance.actors.Add(this);
        }
        public void Move(FixedPointVector3 velocity)
        {
            AddImpulse(velocity);
        }
        public void AddForce()
        {
            forces = FixedPointPhysicsPresenter.GravitationalAcceleration;
            velocity += forces * FixedPointPhysicsPresenter.Instance.DeltaTime;
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
        public void AddConstraints(FixedPointVector3 additionalConstraint)
        {
            if (constraint == FixedPointVector3.zero)
            {
                constraint = additionalConstraint;
            }
            else
            {
                var constraintNormal = constraint.normalized;
                var magnitude = constraint.magnitude;
                var dot = FixedPointVector3.Dot(additionalConstraint, constraintNormal);
                if (dot > magnitude)
                {
                    constraint = additionalConstraint;
                }
                else if (dot > 0)
                {
                    constraint = constraint + additionalConstraint - constraintNormal * dot;
                }
                else
                {
                    constraint = constraint + additionalConstraint;
                }
            }
        }
        public void OnUpdate()
        {
            fixedPointTransform.fixedPointPosition += impulse;
            transform.position = fixedPointTransform.fixedPointPosition.ToVector3();
        }
    }
}
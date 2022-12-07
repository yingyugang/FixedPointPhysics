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
        [HideInInspector]
        public FixedPoint64 friction = 0.1;

        public FixedPointVector3 velocity;
        public FixedPointVector3 currentForce;

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

        public void AddForce(FixedPointVector3 force)
        {
            currentForce += force;
        }

        public void AddForce()
        {
            forces = currentForce + FixedPointPhysicsPresenter.GravitationalAcceleration;
            velocity += forces * FixedPointPhysicsPresenter.Instance.DeltaTime;
            currentForce = FixedPointVector3.zero;
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
            fixedPointTransform.fixedPointPosition += impulse + velocity * FixedPointPhysicsPresenter.Instance.DeltaTime;
            impulse = FixedPointVector3.zero;
            transform.position = fixedPointTransform.fixedPointPosition.ToVector3();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere((fixedPointTransform.fixedPointPosition + new FixedPointVector3(0, radius, 0)).ToVector3(), radius.AsFloat());
                Gizmos.color = Color.white;
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position + new Vector3(0, radius.AsFloat(), 0), radius.AsFloat());
                Gizmos.color = Color.white;
            }
        }
    }
}
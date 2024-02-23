using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;

namespace BlueNoah.PhysicsEngine
{
    public sealed class FPRigidbody : FastListItem
    {
        public bool enable;
        public bool useGravity { get; set; } = true;
        private FixedPoint64 _mass = 1;
        public FixedPoint64 invMass = 1;
        public FixedPoint64 mass {
            get => _mass;
            set
            {
                _mass = value;
                if (_mass == 0)
                {
                    invMass = 0;
                }
                else
                {
                    invMass = 1 / _mass;
                }
            }
        } 
        public FixedPointVector3 velocity;
        public FixedPointVector3 force;//Sum of all forces;
        private FixedPointVector3 deltaMove;
        public FixedPoint64 cor = 0.5;//Coefficient of restitution;
        public bool constrain;
        private readonly FixedPoint64 damping = 1;
        private FixedPointVector3 constraint;
        private List<FPCollision> collisions = new List<FPCollision>();
        public int targetTargetMask { get; set; } = 1 << 0;
        public FPSphereCollider collider { get; private set; }
        public FPTransform transform { get; private set; }
        public FPRigidbody(FPSphereCollider collider, FPTransform transform)
        {
            this.collider = collider;
            this.transform = transform;
            FPPhysicsPresenter.Instance.AddRigidbody(this);
        }
        public void FindCollisionFeatures(FPRigidbody ra,FPRigidbody rb)
        {
            
        }
        public void ApplyImpulse(FPRigidbody ra,FPRigidbody rb,FPCollision collision,int C)
        {
            //Linear Velocity
            var invMass1 = ra.invMass;
            var invMass2 = rb.invMass;
            var invMassSum = invMass1 + invMass2;
            if (invMassSum == 0)
            {
                return;
            }
            //Relative velocity
            var relativeVel = rb.velocity - ra.velocity;
            //Relative collision normal
            var relativeNorm = collision.normal;
            //Moving away from each other? Do nothing!
            if (FixedPointVector3.Dot(relativeVel,relativeNorm) > 0)
            {
                return;
            }
            var e = FixedPointMath.Min(ra.cor,rb.cor);
            var numerator = -(1 + e) * FixedPointVector3.Dot(relativeVel,relativeNorm);
            var j = numerator / invMassSum;
            var impulse = relativeNorm * j;
            ra.velocity = ra.velocity - impulse * invMass1;
            rb.velocity = rb.velocity + impulse * invMass2;
        }
        public void AddLinearImpulse(FixedPointVector3 impulse)
        {
            velocity += impulse;
        }
        public void ApplyForces()
        {
            if (useGravity)
            {
                force = FPPhysicsPresenter.GravitationalAcceleration * mass;
            }
        }
        public void SolveConstraints()
        {
            var count = FPPhysicsPresenter.Instance.fpOctree.OverlaySphereCollision(transform.position ,collider.radius,ref collisions);
            for (var i = 0; i < count; i++)
            {
                if (collisions[i].collider == collider || !collisions[i].hit) continue;
                AddConstraints(collisions[i].normal * (collisions[i].depth * 2));
                AdjustVelocityByCollision(collisions[i].normal, collisions[i].collider.rebound);
            }
            count = FPPhysicsPresenter.Instance.fpOctree.OverlayCharacterWithSphere(this.collider, ref collisions);
            for (var i = 0; i < count; i++)
            {
                if (!collisions[i].hit) continue;
                AddConstraints(-collisions[i].normal * (collisions[i].depth));
                AdjustVelocityByCollision(collisions[i].normal, 0);
            }

            transform.position += constraint;
            deltaMove += constraint;
            if (!constrain && deltaMove.sqrMagnitude > FixedPoint64.EN8)
            {
                var cross = FixedPointVector3.Cross(deltaMove.normalized, FixedPointVector3.up);
                var radian = deltaMove.magnitude * collider.invRadius * FixedPoint64.Rad2Deg;
                //注意
                //Rotate by local axis: transform.rotation =  transform.rotation * FixedPointQuaternion.AngleAxis(-radian, cross.normalized);
                //Rotate by world axis: transform.rotation = FixedPointQuaternion.AngleAxis(-radian, cross.normalized) * transform.rotation;
                transform.rotation = FixedPointQuaternion.AngleAxis(-radian, cross.normalized) * transform.rotation;
            }
            constraint = FixedPointVector3.zero;
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
            if (FPPhysicsPresenter.Instance.fpOctree.IsOutOfBound(transform.position))
            {
                return;
            }
            var acceleration = force * invMass;
            velocity = velocity + acceleration * FPPhysicsPresenter.Instance.DeltaTime;
            deltaMove = velocity * FPPhysicsPresenter.Instance.DeltaTime;
            transform.position += deltaMove;
        }

        private void AdjustVelocityByCollision( FixedPointVector3 constraintNormal,FixedPoint64 rebound)
        {
            var dot = FixedPointVector3.Dot(velocity, constraintNormal);
            if (dot < 0)
            {
                velocity = velocity - constraintNormal * dot * (1 + rebound);
            }
            velocity *= damping;
        }

        public int index { get; set; }
    }
}
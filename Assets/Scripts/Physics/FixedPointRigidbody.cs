using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointRigidbody
    {
        public bool useGravity { get; set; } = true;
        FixedPoint64 _mass = 1;
        public FixedPoint64 invMass = 1;
        public FixedPoint64 mass {
            get
            {
                return _mass;
            }
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
        public FixedPoint64 cor = 0.5;//Coefficient of restitution;
        readonly FixedPoint64 damping = 0.98;

        public FixedPointSphereCollider collider { get; private set; }
        public FixedPointTransform transform { get; private set; }
        public int targetTargetMask { get; set; } = 1 << 0;
        public FixedPointRigidbody(FixedPointSphereCollider collider, FixedPointTransform transform)
        {
            this.collider = collider;
            this.transform = transform;
            FixedPointPhysicsPresenter.Instance.AddRigidbody(this);
        }

        public virtual void FindCollisionFeatures(FixedPointRigidbody ra,FixedPointRigidbody rb)
        {
            
        }

        public virtual void ApplyImpulse(FixedPointRigidbody ra,FixedPointRigidbody rb,FixedPointCollision collision,int C)
        {
            //Linear Velocity
            var invMass1 = ra.invMass;
            var invMass2 = rb.invMass;
            var invMassSum = invMass1 + invMass2;
            if (invMassSum == 0)
            {
                return;
            }
            //Relative velicuty
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

        public virtual void ApplyForces()
        {
            if (useGravity)
            {
                force = FixedPointPhysicsPresenter.GravitationalAcceleration * mass;
            }
        }

        public virtual void SolveConstraints()
        {

        }

        public void OnUpdate()
        {
            if (FixedPointPhysicsPresenter.Instance.fixedPointOctree.IsOutOfBound(transform.fixedPointPosition))
            {
                return;
            }
            var acceleration = force * invMass;
            velocity = velocity + acceleration * FixedPointPhysicsPresenter.Instance.DeltaTime;
            velocity = velocity * damping;
            transform.fixedPointPosition += velocity * FixedPointPhysicsPresenter.Instance.DeltaTime;

            /*
            if (useGravity)
            {
                //s=V0*t + a*t*t/2
                var deltaDistance = FixedPointPhysicsPresenter.Instance.DeltaTime * FixedPointPhysicsPresenter.Instance.DeltaTime * (GravitationalAcceleration + force);
                var prodict = velocity * FixedPointPhysicsPresenter.Instance.DeltaTime + deltaDistance + transform.fixedPointPosition ;
                var colliders = FixedPointPhysicsPresenter.Instance.fixedPointOctree.OverlapSphere(prodict,collider.radius, targetTargetMask);
                FixedPoint64 friction = 0;
                foreach (var item in colliders)
                {
                    if (item!=collider)
                    {
                        if (item.colliderType == ColliderType.AABB)
                        {
                            var aabb = (FixedPointAABBCollider)item;
                            var pos = FixedPointIntersection.ClosestPointWithAABBAndSphere(prodict, aabb.min, aabb.max);
                            var deltaDis = (pos - prodict).normalized * collider.radius - (pos - prodict);
                            prodict -= deltaDis;
                            friction += this.friction;
                        }
                        else if (item.colliderType == ColliderType.Sphere)
                        {
                            var sphere = (FixedPointSphereCollider)item;
                            var normal = (sphere.position - prodict).normalized;
                            var penetration = (collider.radius + sphere.radius) - (prodict - sphere.position).magnitude;
                            prodict -= normal * penetration;
                            friction += this.friction;
                        }
                    }
                }
                velocity = (prodict - transform.fixedPointPosition) / FixedPointPhysicsPresenter.Instance.DeltaTime;
                var magnitude = velocity.magnitude;
                var d = velocity.normalized;
                velocity = d * FixedPointMath.Max(0, magnitude - friction);
                transform.fixedPointPosition = prodict;
            }
            */

        }
    }
}
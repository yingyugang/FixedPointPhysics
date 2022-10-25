using BlueNoah.Math.FixedPoint;
using PE.Grast.Battle;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointRigidbody
    {
        public readonly FixedPointVector3 GravitationalAcceleration = new FixedPointVector3(0, -1, 0) * 9.8;
        public bool useGravity { get; set; } = true;
        public FixedPointVector3 velocity { get; private set; }
        public FixedPointVector3 force { get; set; }
        public FixedPointSphereCollider collider { get; private set; }
        public FixedPointTransform transform { get; private set; }
        public FixedPoint64 friction { get; set; }
        public int targetTargetMask { get; set; } = 1 << 0;
        public FixedPointRigidbody(FixedPointSphereCollider collider, FixedPointTransform transform)
        {
            this.collider = collider;
            this.transform = transform;
            FixedPointPhysicsPresenter.Instance.AddRigidbody(this);
        }

        public void OnUpdate()
        {
            if (useGravity)
            {
                if (FixedPointPhysicsPresenter.Instance.fixedPointOctree.IsOutOfBound(transform.fixedPointPosition))
                {
                    return;
                }
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
                            var normal = (sphere.fixedPointTransform.fixedPointPosition - prodict).normalized;
                            var penetration = (collider.radius + sphere.radius) - (prodict - sphere.fixedPointTransform.fixedPointPosition).magnitude;
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
        }
    }
}
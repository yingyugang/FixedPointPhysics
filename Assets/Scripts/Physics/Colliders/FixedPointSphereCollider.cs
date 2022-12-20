using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointSphereCollider : FixedPointCollider
    {
        public FixedPoint64 radius { get; set; } = 2;

        public FixedPointSphereCollider(FixedPointTransform transform)
        {
            fixedPointTransform = transform;
            colliderType = ColliderType.Sphere;
            if (Application.isPlaying)
            {
                FixedPointPhysicsPresenter.Instance.fixedPointOctree.AddCollider(this);
            }
        }

        public FixedPointSphereCollider(FixedPointTransform transform,FixedPoint64 radius)
        {
            fixedPointTransform = transform;
            this.radius = radius;
            colliderType = ColliderType.Sphere;
            if (Application.isPlaying)
            {
                FixedPointPhysicsPresenter.Instance.fixedPointOctree.AddCollider(this);
            }
        }

        public override void UpdateCollider()
        {
            if (Application.isPlaying && FixedPointPhysicsPresenter.Instance.fixedPointOctree != null)
            {
                FixedPointPhysicsPresenter.Instance.fixedPointOctree.UpdateCollider(this);
            }
        }
    }
}


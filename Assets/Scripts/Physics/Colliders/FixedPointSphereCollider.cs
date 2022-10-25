using BlueNoah.Math.FixedPoint;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointSphereCollider : FixedPointCollider
    {
        public FixedPoint64 radius { get; set; } = 2;
        public FixedPointVector3 size { get; private set; }
        public FixedPointVector3 halfSize { get; private set; }

        public FixedPointSphereCollider(FixedPointTransform transform)
        {
            fixedPointTransform = transform;
            min = fixedPointTransform.fixedPointPosition - halfSize;
            max = fixedPointTransform.fixedPointPosition + halfSize;
            colliderType = ColliderType.Sphere;
        }

        public FixedPointSphereCollider(FixedPointTransform transform,FixedPoint64 radius)
        {
            fixedPointTransform = transform;
            this.radius = radius;
            halfSize = new FixedPointVector3(radius, radius, radius);
            size = halfSize * 2;
            min = fixedPointTransform.fixedPointPosition - halfSize;
            max = fixedPointTransform.fixedPointPosition + halfSize;
        }

        public override void UpdateCollider()
        {
            if (FixedPointPhysicsPresenter.Instance.fixedPointOctree != null)
            {
                FixedPointPhysicsPresenter.Instance.fixedPointOctree.UpdateCollider(this);
                min = fixedPointTransform.fixedPointPosition - halfSize;
                max = fixedPointTransform.fixedPointPosition + halfSize;
            }
        }
    }
}


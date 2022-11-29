using BlueNoah.Math.FixedPoint;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointTriangleCollider : FixedPointCollider
    {
        public FixedPointVector3 a;
        public FixedPointVector3 b;
        public FixedPointVector3 c;

        FixedPointVector3 localMin;
        FixedPointVector3 localMax;

        private FixedPointTriangleCollider()
        {

        }

        public FixedPointTriangleCollider(FixedPointTransform transform, FixedPointVector3 a, FixedPointVector3 b, FixedPointVector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            fixedPointTransform = transform;
            localMin = FixedPointVector3.Min(a, FixedPointVector3.Min(b,c));
            localMax = FixedPointVector3.Max(a, FixedPointVector3.Max(b, c));
            colliderType = ColliderType.Triangle;
        }


        public override void UpdateCollider()
        {
            if (FixedPointPhysicsPresenter.Instance.fixedPointOctree != null)
            {
                FixedPointPhysicsPresenter.Instance.fixedPointOctree.UpdateCollider(this);
                min = position + localMin;
                max = position + localMax;
            }
        }
    }
}
using BlueNoah.Math.FixedPoint;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointOBBCollider : FixedPointCollider
    {
        FixedPointVector3 _size;
        public FixedPointVector3 size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                halfSize = _size / 2;
            }
        }
        public FixedPointVector3 halfSize { get; private set; }
        //Restrict none param construction
        private FixedPointOBBCollider() { }

        public FixedPointOBBCollider(FixedPointTransform transform)
        {
            fixedPointTransform = transform;
            min = fixedPointTransform.fixedPointPosition - halfSize;
            max = fixedPointTransform.fixedPointPosition + halfSize;
            colliderType = ColliderType.OBB;
        }
        public FixedPointOBBCollider(FixedPointTransform transform, FixedPointVector3 size)
        {
            fixedPointTransform = transform;
            this.size = size;
            halfSize = size / 2;
            min = fixedPointTransform.fixedPointPosition - halfSize;
            max = fixedPointTransform.fixedPointPosition + halfSize;
            colliderType = ColliderType.OBB;
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
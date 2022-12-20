using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointOBBCollider : FixedPointCollider
    {
        FixedPointVector3 _size;
        public FixedPoint64 radius;
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
                radius = FixedPointMath.Max(FixedPointMath.Max(halfSize.x, halfSize.y), halfSize.z);
            }
        }
        public FixedPointVector3 halfSize { get; private set; }
        //Restrict none param construction
        private FixedPointOBBCollider() { }

        public FixedPointOBBCollider(FixedPointTransform transform)
        {
            fixedPointTransform = transform;
            min = position - halfSize;
            max = position + halfSize;
            colliderType = ColliderType.OBB;
            if (Application.isPlaying)
            {
                FixedPointPhysicsPresenter.Instance.fixedPointOctree.AddCollider(this);
            }
        }
        public FixedPointOBBCollider(FixedPointTransform transform, FixedPointVector3 size)
        {
            fixedPointTransform = transform;
            this.size = size;
            halfSize = size / 2;
            min = position - halfSize;
            max = position + halfSize;
            colliderType = ColliderType.OBB;
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
                min = position - halfSize;
                max = position + halfSize;
            }
        }
    }
}
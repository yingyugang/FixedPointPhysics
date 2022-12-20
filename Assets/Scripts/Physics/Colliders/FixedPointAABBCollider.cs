using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointAABBCollider : FixedPointCollider
    {
        /*
        public FixedPointVector3 min
        {
            get
            {
                return fixedPointTransform.fixedPointPosition - halfSize;
            }
        }
        public FixedPointVector3 max
        {
            get
            {
                return fixedPointTransform.fixedPointPosition + halfSize;
            }
        }*/

        FixedPointVector3 _size;
        public FixedPointVector3 size { 
            get {
                return _size;
            } 
            set {
                _size = value;
                halfSize = _size / 2;
            } 
        }
        public FixedPointVector3 halfSize { get; private set; }

        public FixedPointAABBCollider(FixedPointTransform transform)
        {
            fixedPointTransform = transform;
            min = position - halfSize;
            max = position + halfSize;
            colliderType = ColliderType.AABB;
            if (Application.isPlaying)
            {
                FixedPointPhysicsPresenter.Instance.fixedPointOctree.AddCollider(this);
            }
        }
        public FixedPointAABBCollider(FixedPointTransform transform,FixedPointVector3 size)
        {
            fixedPointTransform = transform;
            this.size = size;
            halfSize = size / 2;
            min = position - halfSize;
            max = position + halfSize;
            colliderType = ColliderType.AABB;
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
using BlueNoah.Math.FixedPoint;
using UnityEngine;


namespace BlueNoah.PhysicsEngine
{
    public class TestSphereAABBCollision : MonoBehaviour
    {
        public FixedPointAABBColliderPresenter fixedPointAABBColliderPresenter;
        public FixedPointSphereColliderPresenter fixedPointSphereColliderPresenter;
        public Transform target;
        FixedPointVector3 pos;
        //public Transform target1;
        private void FixedUpdate()
        {
            FixedPointPhysicsPresenter.Instance.OnUpdate();
        }

        private void Update()
        {
            var trans = fixedPointSphereColliderPresenter.fixedPointSphereCollider.fixedPointTransform.fixedPointPosition;
            var radius = fixedPointSphereColliderPresenter.fixedPointSphereCollider.radius;
            var colliders = FixedPointPhysicsPresenter.OverlapSphere(new FixedPointVector3(target.position), radius, 1 << 0);
            foreach (var item in colliders)
            {
                if (item.colliderType == ColliderType.AABB)
                {
                    var aabb = (FixedPointAABBCollider)item;
                    pos = FixedPointIntersection.ClosestPointWithAABBAndSphere(new FixedPointVector3(target.position),aabb.min, aabb.max);
                    Debug.Log(pos);
                    //target1.position = pos.ToVector3();
                }
            }
            //var colliders = FixedPointPhysicsPresenter.OverlapSphere(trans, radius,1 << 0);
        }
    }
}
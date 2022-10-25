using BlueNoah.Math.FixedPoint;
using TMPro;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionPointExample : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI result;
        [SerializeField]
        GameObject target;
        [SerializeField]
        FixedPointAABBColliderPresenter aabb;
        [SerializeField]
        FixedPointOBBColliderPresenter obb;
        [SerializeField]
        FixedPointSphereColliderPresenter sphere;

        [SerializeField]
        GameObject closest;
        [SerializeField]
        GameObject closest1;
        [SerializeField]
        GameObject closest2;

        void Update()
        {
            var pos = new FixedPointVector3(target.transform.position);
            result.text = "Intersection : false";
            if (FixedPointIntersection.PointInAABB(pos,aabb.fixedPointAABBCollider.min ,aabb.fixedPointAABBCollider.max))
            {
                result.text = "Intersection : true";
            }
            if (FixedPointIntersection.PointInOBB(pos, obb.fixedPointOBBCollider.fixedPointTransform.fixedPointPosition, obb.fixedPointOBBCollider.halfSize, obb.fixedPointOBBCollider.orientation))
            {
                result.text = "Intersection : true";
            }
            if (FixedPointIntersection.PointInSphere(pos, sphere.fixedPointSphereCollider.fixedPointTransform.fixedPointPosition, sphere.fixedPointSphereCollider.radius))
            {
                result.text = "Intersection : true";
            }
            closest.transform.position = FixedPointIntersection.ClosestPointWithPointAndAABB(pos, aabb.fixedPointAABBCollider.min, aabb.fixedPointAABBCollider.max).ToVector3();
            closest1.transform.position = FixedPointIntersection.ClosestPointWithPointAndOBB(pos, obb.fixedPointOBBCollider.fixedPointTransform.fixedPointPosition, obb.fixedPointOBBCollider.halfSize, obb.fixedPointOBBCollider.orientation).ToVector3();
            closest2.transform.position = FixedPointIntersection.ClosestPointWithPointAndSphere(pos, sphere.fixedPointSphereCollider.fixedPointTransform.fixedPointPosition, sphere.fixedPointSphereCollider.radius).ToVector3();
        }
    }
}
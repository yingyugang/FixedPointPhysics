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
        FixedPointTriangleColliderPresenter triangle;

        [SerializeField]
        GameObject planeObj;
        FixedPointPlane plane;

        [SerializeField]
        GameObject closest;
        [SerializeField]
        GameObject closest1;
        [SerializeField]
        GameObject closest2;
        [SerializeField]
        GameObject closest3;
        [SerializeField]
        GameObject closest4;
        void Update()
        {
            plane = new FixedPointPlane(new FixedPointVector3(planeObj.transform.up) ,Vector3.Dot(planeObj.transform.position, planeObj.transform.up) );
            var pos = new FixedPointVector3(target.transform.position);
            result.text = "Intersection : false";
            if (FixedPointIntersection.PointInAABB(pos,aabb.fixedPointAABBCollider.min ,aabb.fixedPointAABBCollider.max))
            {
                result.text = "Intersection : true";
            }
            if (FixedPointIntersection.PointInOBB(pos, obb.fixedPointOBBCollider.position, obb.fixedPointOBBCollider.halfSize, obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix))
            {
                result.text = "Intersection : true";
            }
            if (FixedPointIntersection.PointInSphere(pos, sphere.fixedPointSphereCollider.position, sphere.fixedPointSphereCollider.radius))
            {
                result.text = "Intersection : true";
            }
            if (FixedPointIntersection.PointInTriangle(pos, triangle.fixedPointTriangleCollider))
            {
                result.text = "Intersection : true";
            }
            if (FixedPointIntersection.PointOnPlane(pos, plane))
            {
                result.text = "Intersection : true";
            }
            closest.transform.position = FixedPointIntersection.ClosestPointWithPointAndAABB(pos, aabb.fixedPointAABBCollider.min, aabb.fixedPointAABBCollider.max).ToVector3();
            closest1.transform.position = FixedPointIntersection.ClosestPointWithPointAndOBB(pos, obb.fixedPointOBBCollider.position, obb.fixedPointOBBCollider.halfSize, obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix).ToVector3();
            closest2.transform.position = FixedPointIntersection.ClosestPointWithPointAndSphere(pos, sphere.fixedPointSphereCollider.position, sphere.fixedPointSphereCollider.radius).ToVector3();
            closest3.transform.position = FixedPointIntersection.ClosestPointWithPointAndTriangle(pos, triangle.fixedPointTriangleCollider).ToVector3();
            closest4.transform.position = FixedPointIntersection.ClosestPointWithPointAndPlane(pos, plane).ToVector3();

        }
    }
}
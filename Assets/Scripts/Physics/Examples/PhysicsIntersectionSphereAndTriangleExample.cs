using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionSphereAndTriangleExample : MonoBehaviour
    {
        public FixedPointSphereColliderPresenter sphere;
        public FixedPointTriangleColliderPresenter triangle;
        public GameObject hitObj;

        void Update()
        {
            sphere.fixedPointSphereCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(sphere.transform.position);
            sphere.fixedPointSphereCollider.radius = sphere.transform.localScale.x / 2;
            triangle.fixedPointTriangleCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(triangle.transform.position);
            triangle.fixedPointTriangleCollider.a = new FixedPointVector3(triangle.vertexA) / 1000;
            triangle.fixedPointTriangleCollider.b = new FixedPointVector3(triangle.vertexB) / 1000;
            triangle.fixedPointTriangleCollider.c = new FixedPointVector3(triangle.vertexC) / 1000;
            var collision = FixedPointIntersection.IntersectWithSphereAndTriangle(sphere.fixedPointSphereCollider.position, sphere.fixedPointSphereCollider.radius, triangle.fixedPointTriangleCollider);
            hitObj.SetActive(collision.hit);
            if (collision.hit)
            {
                hitObj.transform.forward = collision.normal.ToVector3();
                hitObj.transform.position = collision.closestPoint.ToVector3();
            }
            /*
            aabb.fixedPointAABBCollider.size = new FixedPointVector3(aabb.transform.localScale);
            aabb.fixedPointAABBCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(aabb.transform.position);
            obb.fixedPointOBBCollider.size = new FixedPointVector3(obb.transform.localScale);
            obb.fixedPointOBBCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(obb.transform.position);
            obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix = FixedPointMatrix.CreateFromYawPitchRoll(obb.transform.eulerAngles.y * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.x * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.z * FixedPointMath.Deg2Rad);
            var intersect = FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(aabb.fixedPointAABBCollider.min, aabb.fixedPointAABBCollider.max, obb.fixedPointOBBCollider);
            */
        }
    }
}
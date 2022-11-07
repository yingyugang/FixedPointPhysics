using BlueNoah.Math.FixedPoint;
using TMPro;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionAABBAndOBBExample : MonoBehaviour
    {
        public TextMeshProUGUI resultTxt;
        public FixedPointAABBColliderPresenter aabb;
        public FixedPointOBBColliderPresenter obb;

        void Update()
        {
            aabb.fixedPointAABBCollider.size = new FixedPointVector3(aabb.transform.localScale);
            aabb.fixedPointAABBCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(aabb.transform.position);
            obb.fixedPointOBBCollider.size = new FixedPointVector3(obb.transform.localScale);
            obb.fixedPointOBBCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(obb.transform.position);
            obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix = FixedPointMatrix.CreateFromYawPitchRoll(obb.transform.eulerAngles.y * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.x * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.z * FixedPointMath.Deg2Rad);
            var intersect = FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(aabb.fixedPointAABBCollider.min, aabb.fixedPointAABBCollider.max, obb.fixedPointOBBCollider);
            resultTxt.text = "Intersection : " + intersect;
        }
    }
}
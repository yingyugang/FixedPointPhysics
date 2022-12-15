using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class PhysicsOverlapBoxExample : MonoBehaviour
    {
        public GameObject hitGo;
        public FixedPointOBBColliderPresenter obb;

        void Update()
        {
            obb.fixedPointOBBCollider.size = new FixedPointVector3(obb.transform.localScale);
            obb.fixedPointOBBCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(obb.transform.position);
            obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix = FixedPointMatrix.CreateFromYawPitchRoll(obb.transform.eulerAngles.y * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.x * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.z * FixedPointMath.Deg2Rad);

            var collisions = FixedPointPhysicsPresenter.OverlayBoxCollision(obb.fixedPointOBBCollider.position,obb.fixedPointOBBCollider.halfSize,obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix);
            hitGo.SetActive(false);
            foreach (var item in collisions)
            {
                if (item.hit)
                {
                    hitGo.SetActive(true);
                    hitGo.transform.position = item.contactPoint.ToVector3();
                    hitGo.transform.forward = item.normal.ToVector3();
                    Debug.DrawRay(item.contactPoint.ToVector3(), item.normal.ToVector3());
                }
            }
        }
    }
}
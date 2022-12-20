using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class PhysicsOverlapBoxExample : MonoBehaviour
    {
        public GameObject hitGo;
        public FixedPointOBBColliderPresenter obb;
        public FixedPointSphereColliderPresenter sphere;
        public FixedPointSphereColliderPresenter sphere1;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                obb.fixedPointOBBCollider.size = new FixedPointVector3(obb.transform.localScale);
                obb.fixedPointOBBCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(obb.transform.position);
                obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix = FixedPointMatrix.CreateFromYawPitchRoll(obb.transform.eulerAngles.y * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.x * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.z * FixedPointMath.Deg2Rad);
                sphere.fixedPointSphereCollider.radius = sphere.transform.localScale.x / 2;
                sphere.fixedPointSphereCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(sphere.transform.position);
                sphere1.fixedPointSphereCollider.radius = sphere1.transform.localScale.x / 2;
                sphere1.fixedPointSphereCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(sphere1.transform.position);
                var collisions = FixedPointPhysicsPresenter.OverlayBoxCollision(obb.fixedPointOBBCollider.position, obb.fixedPointOBBCollider.halfSize, obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix);
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
}
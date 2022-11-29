using BlueNoah.Math.FixedPoint;
using TMPro;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionSphereExample : MonoBehaviour
    {
        public TextMeshProUGUI resultTxt;
        public FixedPointSphereColliderPresenter sphere;
        public FixedPointOBBColliderPresenter obb;
        public GameObject arrow;
        [SerializeField]
        GameObject planeObj;
        FixedPointPlane plane;
        public GameObject arrow1;

        private void Awake()
        {
            plane = new FixedPointPlane(new FixedPointVector3(planeObj.transform.up), Vector3.Dot(planeObj.transform.position, planeObj.transform.up));
        }

        void Update()
        {
            sphere.fixedPointSphereCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(sphere.transform.position);
            sphere.fixedPointSphereCollider.radius = sphere.transform.localScale.x / 2;
            obb.fixedPointOBBCollider.size = new FixedPointVector3(obb.transform.localScale);
            obb.fixedPointOBBCollider.fixedPointTransform.fixedPointPosition = new FixedPointVector3(obb.transform.position);
            //obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix = FixedPointMatrix.CreateFromYawPitchRoll(obb.transform.eulerAngles.y * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.x * FixedPointMath.Deg2Rad, obb.transform.eulerAngles.z * FixedPointMath.Deg2Rad);
            obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix = FixedPointMatrix.CreateFromQuaternion(FixedPointQuaternion.Euler(new FixedPointVector3(obb.transform.eulerAngles)));
            var intersect = FixedPointIntersection.IntersectWithSphereAndOBB(sphere.fixedPointSphereCollider, obb.fixedPointOBBCollider);
            arrow.gameObject.SetActive(intersect.hit);
            if (intersect.hit)
            {
                arrow.transform.position = intersect.closestPoint.ToVector3();
                arrow.transform.forward = intersect.normal.ToVector3();
            }
            var collision = FixedPointIntersection.IntersectWithSphereAndPlane(sphere.fixedPointSphereCollider.position, sphere.fixedPointSphereCollider.radius, plane);
            arrow1.gameObject.SetActive(collision.hit);
            if (collision.hit)
            {
                arrow1.transform.position = collision.closestPoint.ToVector3();
                arrow1.transform.forward = collision.normal.ToVector3();
            }
        }
    }
}
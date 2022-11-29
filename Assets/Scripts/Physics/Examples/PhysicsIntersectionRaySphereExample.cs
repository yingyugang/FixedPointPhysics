using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionRaySphereExample : MonoBehaviour
    {
        Vector3 origin;
        Vector3 direction;
        public FixedPointSphereColliderPresenter sphere;

        public Transform originObj;
        public Transform directionObj;
        GameObject go1;
        void Update()
        {
            FixedPointCollision fixedPointRaycastHit;
            if (Input.GetMouseButtonDown(0))
            {
                var origin = new FixedPointVector3(Camera.main.transform.position);
                var mousePosition = Input.mousePosition;
                mousePosition.z = 10;
                Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
                var direction = (position - Camera.main.transform.position).normalized;
                this.origin = origin.ToVector3();
                this.direction = direction;
                if (FixedPointIntersection.IntersetWithRayAndSphereFixedPoint(origin, new FixedPointVector3(direction),1000, sphere.fixedPointSphereCollider.position, sphere.fixedPointSphereCollider.radius , out fixedPointRaycastHit))
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = fixedPointRaycastHit.closestPoint.ToVector3();
                    Destroy(go, 3);
                }
            }
            if (FixedPointIntersection.IntersetWithRayAndSphereFixedPoint(new FixedPointVector3(originObj.position), new FixedPointVector3((directionObj.position - originObj.position).normalized), 1000, sphere.fixedPointSphereCollider.position, sphere.fixedPointSphereCollider.radius, out fixedPointRaycastHit))
            {
                if (go1 == null)
                    go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go1.transform.position = fixedPointRaycastHit.closestPoint.ToVector3();
                go1.SetActive(true);
            }
            else
            {
                go1.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + direction * 100);
        }
    }
}
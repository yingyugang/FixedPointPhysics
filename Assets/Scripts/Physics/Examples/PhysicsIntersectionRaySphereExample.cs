using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionRaySphereExample : MonoBehaviour
    {
        Vector3 origin;
        Vector3 direction;
        public FixedPointSphereColliderPresenter sphere;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var origin = new FixedPointVector3(Camera.main.transform.position);
                var mousePosition = Input.mousePosition;
                mousePosition.z = 10;
                Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
                var direction = (position - Camera.main.transform.position).normalized;
                FixedPointCollision fixedPointRaycastHit;
                this.origin = origin.ToVector3();
                this.direction = direction;
                if (FixedPointIntersection.IntersetionWithRayAndSphereFixedPoint(origin, new FixedPointVector3(direction),1000, sphere.fixedPointSphereCollider.fixedPointTransform.fixedPointPosition, sphere.fixedPointSphereCollider.radius , out fixedPointRaycastHit))
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = fixedPointRaycastHit.point.ToVector3();
                    Destroy(go,3);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + direction * 100);
        }
    }
}
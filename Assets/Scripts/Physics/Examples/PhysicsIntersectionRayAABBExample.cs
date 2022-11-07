using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionRayAABBExample : MonoBehaviour
    {
        Vector3 origin;
        Vector3 direction;
        public FixedPointAABBColliderPresenter aabb;

        // Update is called once per frame
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
                if (FixedPointIntersection.IntersectionWithRayAndAABBFixedPointA(origin, new FixedPointVector3(direction), aabb.fixedPointAABBCollider.min, aabb.fixedPointAABBCollider.max, out fixedPointRaycastHit))
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = fixedPointRaycastHit.point.ToVector3();
                    Destroy(go, 3);
                }

                FixedPointVector3 fixedPointVector3;
                var dis = FixedPointIntersection.IntersectionWithRayAndAABBFixedPoint(origin, new FixedPointVector3(direction) * 1000, aabb.fixedPointAABBCollider.min, aabb.fixedPointAABBCollider.max, out fixedPointVector3);
                if (dis != FixedPoint64.MaxValue)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = fixedPointVector3.ToVector3();
                    Destroy(go, 3);
                }

                this.origin = origin.ToVector3();
                this.direction = direction;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + direction * 100);
        }
    }
}
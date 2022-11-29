using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionRayOBBExample : MonoBehaviour
    {
        Vector3 origin;
        Vector3 direction;
        public FixedPointOBBColliderPresenter obb;

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
                if (FixedPointIntersection.IntersectWithRayAndOBBFixedPoint(origin, new FixedPointVector3(direction), obb.fixedPointOBBCollider.position,  obb.fixedPointOBBCollider.halfSize, obb.fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix, out fixedPointRaycastHit) > 0)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = fixedPointRaycastHit.closestPoint.ToVector3();
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
using BlueNoah.Math.FixedPoint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsIntersectionRayPlaneExample : MonoBehaviour
    {
        Vector3 origin;
        Vector3 direction;
        public GameObject plane;
        FixedPointPlane fixedPointPlane = new FixedPointPlane();

        void Update()
        {
            fixedPointPlane.normal = new FixedPointVector3(plane.transform.up);
            fixedPointPlane.distance = Vector3.Project(plane.transform.position, plane.transform.up).magnitude;  

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
                if (FixedPointIntersection.IntersectWithRayAndPlaneFixedPoint(origin, new FixedPointVector3(direction), fixedPointPlane.distance, fixedPointPlane.normal, out fixedPointRaycastHit))
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = fixedPointRaycastHit.closestPoint.ToVector3();
                    Destroy(go, 3);
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
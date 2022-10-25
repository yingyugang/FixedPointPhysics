using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class PhysicsRaycastExample : MonoBehaviour
    {
        Vector3 origin;
        Vector3 direction;

        [SerializeField]
        GameObject rayStart;
        [SerializeField]
        GameObject rayEnd;
        FixedPointRaycastHit rayHit;


        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0)) {
                var origin = new FixedPointVector3(Camera.main.transform.position);
                var mousePosition = Input.mousePosition;
                mousePosition.z = 10;
                Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
                var direction = (position - Camera.main.transform.position).normalized;
                FixedPointRaycastHit fixedPointRaycastHit;
                if (FixedPointPhysicsPresenter.Raycast(origin,new FixedPointVector3(direction) , 10000, out fixedPointRaycastHit, 0)) {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = fixedPointRaycastHit.point.ToVector3();
                }
                this.origin = origin.ToVector3();
                this.direction = direction;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + direction * 100);
            if (Application.isPlaying)
            {
                if (FixedPointPhysicsPresenter.Raycast(new FixedPointVector3(rayStart.transform.position), new FixedPointVector3(rayEnd.transform.position - rayStart.transform.position).normalized, 10000, out rayHit, 0))
                {
                    Gizmos.DrawSphere(rayHit.point.ToVector3(), 0.3f);
                }
                Gizmos.DrawLine(rayStart.transform.position, (rayEnd.transform.position - rayStart.transform.position).normalized * 1000);
            }
        }
    }
}
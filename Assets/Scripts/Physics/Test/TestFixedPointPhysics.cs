using BlueNoah.CameraControl;
using BlueNoah.Event;
using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class TestFixedPointPhysics : MonoBehaviour
    {
        public float radius;
        public Transform start;
        public BoxCollider target;

        FixedPointRay fixedPointRay;

        public BoxCollider targetAABB;
        public FixedPointAABBCollider fixedPointAABBCollider;

        private void Awake()
        {
            EasyInput.Instance.AddListener(Event.TouchType.Click, 0, (evt) =>
            {
                IntersectionRayAABB(evt);
                //IntersectionRayPlane(evt);
            });
        }

        private void FixedUpdate()
        {
            FixedPointPhysicsPresenter.Instance.OnUpdate();
            FixedPointPhysicsPresenter.OverlapSphere(new FixedPointVector3(start.position), 0.1, 1 << 0);
        }

        private void Update()
        {
            //TODO
            //fixedPointAABBCollider.position = new FixedPointVector3(fixedPointAABBCollider.transform.position);
            if (Input.GetKeyDown(KeyCode.H))
            {
                FixedPointPhysicsPresenter.OverlapSphere(new FixedPointVector3(start.position),1,1<<0);
            }

        }

        void IntersectionRayAABB(EventData evt)
        {
            RaycastHit raycastHit;
            if (CameraController.Instance.GetWorldPoistionByScreenPos(out raycastHit, evt.currentTouch.touch.position, 1 << 0))
            {
                Debug.Log(raycastHit.point);
            }
            var position0 = evt.currentTouch.touch.position;
            var mousePosition = new Vector3(position0.x, position0.y, 10);
            Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 forward = (position - Camera.main.transform.position).normalized;
            //fixedPointRay = new FixedPointRay(new Math.FixedPoint.FixedPointVector3(Camera.main.transform.position), new Math.FixedPoint.FixedPointVector3(0, -1, 0));
            //var fixedPointAABB = new FixedPointAABB(new Math.FixedPoint.FixedPointVector3(target.bounds.min), new Math.FixedPoint.FixedPointVector3(target.bounds.max));
            Vector3 normal;
            // var intersect = FixedPointIntersection.RayIntercect(Camera.main.transform.position, forward * 10000, target.bounds.min, target.bounds.max, out normal);
            var intersect = FixedPointIntersection.RaycastAABBFloat(target.transform.position + Vector3.up * 1000, -Vector3.up * 10000, target.bounds.min, target.bounds.max, out normal);
            Debug.Log(intersect);
            FixedPointCollision normalFixedPoint;
            var intersectFixed = FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(new FixedPointVector3(Camera.main.transform.position),new FixedPointVector3(forward) * 10000, new FixedPointVector3(targetAABB.bounds.min), new FixedPointVector3(targetAABB.bounds.max), out normalFixedPoint);
            Debug.Log(intersectFixed);
            Debug.Log(normalFixedPoint.normal);
        }

        void Intersection1(EventData evt)
        {
            RaycastHit raycastHit;
            if (CameraController.Instance.GetWorldPoistionByScreenPos(out raycastHit, evt.currentTouch.touch.position, 1 << 0))
            {
                Debug.Log(raycastHit.point);
            }
            var position0 = evt.currentTouch.touch.position;
            var mousePosition = new Vector3(position0.x, position0.y, 10);
            Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 forward = (position - Camera.main.transform.position).normalized;
            //fixedPointRay = new FixedPointRay(new Math.FixedPoint.FixedPointVector3(Camera.main.transform.position), new Math.FixedPoint.FixedPointVector3(0, -1, 0));
            //var fixedPointAABB = new FixedPointAABB(new Math.FixedPoint.FixedPointVector3(target.bounds.min), new Math.FixedPoint.FixedPointVector3(target.bounds.max));
            Vector3 normal;
           // var intersect = FixedPointIntersection.RayIntercect(Camera.main.transform.position, forward * 10000, target.bounds.min, target.bounds.max, out normal);
            var intersect = FixedPointIntersection.RaycastAABBFloat(target.transform.position + Vector3.up * 1000, -Vector3.up * 10000, target.bounds.min, target.bounds.max, out normal);
            Debug.Log(intersect);

            FixedPointCollision normalFixedPoint;
            var intersectFixed = FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(new FixedPointVector3(target.transform.position + Vector3.up * 1000), FixedPointVector3.up * -10000,new FixedPointVector3 (target.bounds.min) , new FixedPointVector3(target.bounds.max) , out normalFixedPoint);
            Debug.Log(intersectFixed);
            Debug.Log(normalFixedPoint.normal);
        }

        void IntersectionRayPlane(EventData evt)
        {
            RaycastHit raycastHit;
            if (CameraController.Instance.GetWorldPoistionByScreenPos(out raycastHit, evt.currentTouch.touch.position, 1 << 0))
            {
                Debug.Log(raycastHit.point);
            }
            var position0 = evt.currentTouch.touch.position;
            var mousePosition = new Vector3(position0.x, position0.y, 10);
            Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 forward = (position - Camera.main.transform.position).normalized;
            Vector3 insertsect1;
            if (FixedPointIntersection.IntersectWithRayAndPlane(Camera.main.transform.position, -forward, -Vector3.up, Vector3.zero, out insertsect1))
            {
                Debug.Log(insertsect1);
            }
            if(Physics.Raycast(Camera.main.transform.position, -forward,out raycastHit))
            {
                Debug.Log(raycastHit.point);
            }
        }
    }
}
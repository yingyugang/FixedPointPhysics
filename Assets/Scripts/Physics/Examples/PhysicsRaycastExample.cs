using BlueNoah.Math.FixedPoint;
using System.Collections;
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

        public int length = 30;
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
                if (FixedPointPhysicsPresenter.Raycast(origin,new FixedPointVector3(direction) , length, out fixedPointRaycastHit, 0)) {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = fixedPointRaycastHit.point.ToVector3();
                    StartCoroutine(_Destroy(go, fixedPointRaycastHit.normal.ToVector3()));
                }
                this.origin = origin.ToVector3();
                this.direction = direction;
            }
        }

        IEnumerator _Destroy(GameObject go, Vector3 normal)
        {
            yield return new WaitForSeconds(0.2f);
            float t = 0;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                go.transform.position += Time.deltaTime * normal * 20;
                yield return null;
            }
            Destroy(go);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + direction * length);
            /*
            if (Application.isPlaying)
            {
                if (FixedPointPhysicsPresenter.Raycast(new FixedPointVector3(rayStart.transform.position), new FixedPointVector3(rayEnd.transform.position - rayStart.transform.position).normalized, 10000, out rayHit, 0))
                {
                    Gizmos.DrawSphere(rayHit.point.ToVector3(), 0.3f);
                }
                Gizmos.DrawLine(rayStart.transform.position, (rayEnd.transform.position - rayStart.transform.position).normalized * length);
            }
            */
        }
    }
}
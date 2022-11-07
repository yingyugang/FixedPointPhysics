using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    [ExecuteInEditMode]
    public class FixedPointSphereColliderPresenter : FixedPointColliderPresenter
    {
        public FixedPointSphereCollider fixedPointSphereCollider { get; private set; }
        public FixedPointRigidbody fixedPointRigidbody { get; private set; }

        [SerializeField]
        int friction = 10;
        [SerializeField]
        bool useRigidbody;
        [SerializeField]
        int radiusInt;


        FixedPointTransform fixedPointTransform;
        private void Awake()
        {
            fixedPointTransform = new FixedPointTransform(OnTransfered);
            fixedPointTransform.fixedPointPosition = new FixedPointVector3(positionInt) / 1000;
            fixedPointSphereCollider = new FixedPointSphereCollider(fixedPointTransform);
            fixedPointCollider = fixedPointSphereCollider;
            fixedPointSphereCollider.radius = new FixedPoint64(radiusInt) / 1000;
            fixedPointSphereCollider.isTrigger = isTrigger;
            fixedPointSphereCollider.layer = layer;
            fixedPointSphereCollider.actorPresenter = gameObject;
            colliderType = fixedPointSphereCollider.colliderType;
            fixedPointTransform.SetFixedPointCollider(fixedPointSphereCollider);
            if (useRigidbody)
            {
                fixedPointRigidbody = new FixedPointRigidbody(fixedPointSphereCollider, fixedPointTransform);
                fixedPointRigidbody.friction = new FixedPoint64(friction) / 1000;
                fixedPointRigidbody.force = new FixedPointVector3(5, 0, 0);
            }
        }
        void OnTransfered()
        {
            transform.position = fixedPointTransform.fixedPointPosition.ToVector3();
        }

#if UNITY_EDITOR
        private void Update()
        {
            positionInt = new Vector3Int((int)(transform.position.x * 1000), (int)(transform.position.y * 1000), (int)(transform.position.z * 1000));
            radiusInt = (int)(transform.localScale.x * 500);
        }
#endif

        private void OnDrawGizmos()
        {
            if (fixedPointSphereCollider != null)
            {
                Gizmos.color = Color.green;
                if (Application.isPlaying)
                {
                    Gizmos.DrawWireSphere(fixedPointSphereCollider.fixedPointTransform.fixedPointPosition.ToVector3(), fixedPointSphereCollider.radius.AsFloat());
                }
                else
                {
                   
                    Gizmos.DrawWireSphere((Vector3)positionInt / 1000, radiusInt / 1000f);
                }
                Gizmos.color = Color.white;
            }
        }
    }
}
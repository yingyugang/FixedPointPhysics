using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    [ExecuteInEditMode]
    public class FixedPointAABBColliderPresenter : FixedPointColliderPresenter
    {
        public FixedPointAABBCollider fixedPointAABBCollider { get; private set; }
        [SerializeField]
        Vector3Int sizeInt;
        [SerializeField]
        Vector3Int positionInt;

        private void Awake()
        {
            var fixedPointTransform = new FixedPointTransform();
            fixedPointTransform.fixedPointPosition = new FixedPointVector3(positionInt) / 1000;
            fixedPointAABBCollider = new FixedPointAABBCollider(fixedPointTransform);
            fixedPointCollider = fixedPointAABBCollider;
            fixedPointAABBCollider.size = new FixedPointVector3(sizeInt) / 1000;
            fixedPointAABBCollider.actorPresenter = gameObject;
            fixedPointTransform.SetFixedPointCollider(fixedPointAABBCollider);
            fixedPointAABBCollider.UpdateCollider();
            fixedPointAABBCollider.isTrigger = isTrigger;
            fixedPointAABBCollider.layer = layer;
            colliderType = fixedPointAABBCollider.colliderType;
        }
#if UNITY_EDITOR
        private void Update()
        {
            positionInt = new Vector3Int((int)(transform.position.x * 1000), (int)(transform.position.y * 1000), (int)(transform.position.z * 1000));
            sizeInt = new Vector3Int((int)(transform.localScale.x * 1000), (int)(transform.localScale.y * 1000), (int)(transform.localScale.z * 1000));
        }
#endif
        private void OnDrawGizmos()
        {
            if (fixedPointAABBCollider!=null)
            {
                Gizmos.color = Color.green;
                if (Application.isPlaying)
                {
                    Gizmos.DrawWireCube(fixedPointAABBCollider.fixedPointTransform.fixedPointPosition.ToVector3(), fixedPointAABBCollider.size.ToVector3());
                }
                else
                {
                    Gizmos.DrawWireCube((Vector3)positionInt / 1000, sizeInt / 1000);
                }
                Gizmos.color = Color.white;
            }
        }
    }
}
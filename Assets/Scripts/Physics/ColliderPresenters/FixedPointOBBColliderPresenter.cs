using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    [ExecuteAlways]
    public class FixedPointOBBColliderPresenter : FixedPointColliderPresenter
    {
        public FixedPointOBBCollider fixedPointOBBCollider { get; private set; }
        [SerializeField]
        Vector3Int sizeInt;
        [SerializeField]
        Vector3Int eulerInt;

        private void Awake()
        {
            var fixedPointTransform = new FixedPointTransform();
            fixedPointTransform.fixedPointPosition = new FixedPointVector3(positionInt) / 1000;
            var radian = new FixedPointVector3(eulerInt) / 1000 * FixedPoint64.Deg2Rad;
            var matrix = FixedPointMatrix.CreateFromYawPitchRoll(radian.y, radian.x, radian.z);
            fixedPointTransform.fixedPointMatrix = matrix;
            fixedPointOBBCollider = new FixedPointOBBCollider(fixedPointTransform, new FixedPointVector3(sizeInt) / 1000);
            fixedPointCollider = fixedPointOBBCollider;
            fixedPointOBBCollider.actorPresenter = gameObject;
            fixedPointTransform.SetFixedPointCollider(fixedPointCollider);
            fixedPointOBBCollider.UpdateCollider();
            fixedPointOBBCollider.isTrigger = isTrigger;
            fixedPointOBBCollider.layer = layer;
            colliderType = fixedPointOBBCollider.colliderType;
        }
#if UNITY_EDITOR
        private void Update()
        {
            positionInt = new Vector3Int((int)(transform.position.x * 1000), (int)(transform.position.y * 1000), (int)(transform.position.z * 1000));
            sizeInt = new Vector3Int((int)(transform.localScale.x * 1000), (int)(transform.localScale.y * 1000), (int)(transform.localScale.z * 1000));
            eulerInt = new Vector3Int((int)(transform.eulerAngles.x * 1000), (int)(transform.eulerAngles.y * 1000), (int)(transform.eulerAngles.z * 1000));
        }
#endif
        private void OnDrawGizmos()
        {
            if (fixedPointOBBCollider != null)
            {
                Gizmos.color = Color.blue;
                var matrix = Gizmos.matrix;
                if (Application.isPlaying)  
                {
                    var eulerAngles = fixedPointOBBCollider.fixedPointTransform.fixedPointMatrix.eulerAngles;
                    //Gizmos.matrix = Matrix4x4.TRS(fixedPointOBBCollider.fixedPointTransform.fixedPointPosition.ToVector3(), Quaternion.Euler(eulerAngles.ToVector3()), fixedPointOBBCollider.size.ToVector3());
                    //Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    Gizmos.matrix = Matrix4x4.TRS((Vector3)positionInt / 1000f, Quaternion.Euler((Vector3)eulerInt / 1000f), (Vector3)sizeInt / 1000f);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                }
                else
                {
                    Gizmos.matrix = Matrix4x4.TRS((Vector3)positionInt / 1000f, Quaternion.Euler((Vector3)eulerInt / 1000f), (Vector3)sizeInt / 1000f);
                    Gizmos.DrawWireCube(Vector3.zero,Vector3.one );
                }
                Gizmos.matrix = matrix;
                Gizmos.color = Color.white;
            }
        }
    }
}
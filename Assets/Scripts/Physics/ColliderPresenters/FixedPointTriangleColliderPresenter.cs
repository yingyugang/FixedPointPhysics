using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    [ExecuteAlways]
    public class FixedPointTriangleColliderPresenter : FixedPointColliderPresenter
    {
        public FixedPointTriangleCollider fixedPointTriangleCollider { get; private set; }
        public Vector3Int vertexA;
        public Vector3Int vertexB;
        public Vector3Int vertexC;
        [SerializeField]
        Vector3Int eulerInt;

        Vector3[] vertices = new Vector3[3];

        private void Awake()
        {
            var fixedPointTransform = new FixedPointTransform();
            fixedPointTransform.fixedPointPosition = new FixedPointVector3(positionInt) / 1000;
            var radian = new FixedPointVector3(eulerInt) / 1000 * FixedPoint64.Deg2Rad;
            var matrix = FixedPointMatrix.CreateFromYawPitchRoll(radian.y, radian.x, radian.z);
            fixedPointTransform.fixedPointMatrix = matrix;
            fixedPointTriangleCollider = new FixedPointTriangleCollider(fixedPointTransform, new FixedPointVector3(vertexA) / 1000, new FixedPointVector3(vertexB) / 1000, new FixedPointVector3(vertexC) / 1000);
            fixedPointCollider = fixedPointTriangleCollider;
            fixedPointTriangleCollider.actorPresenter = gameObject;
            fixedPointTransform.SetFixedPointCollider(fixedPointCollider);
            fixedPointTriangleCollider.UpdateCollider();
            fixedPointTriangleCollider.isTrigger = isTrigger;
            fixedPointTriangleCollider.layer = layer;
            colliderType = fixedPointTriangleCollider.colliderType;
        }

#if UNITY_EDITOR
        private void Update()
        {
            positionInt = new Vector3Int((int)(transform.position.x * 1000), (int)(transform.position.y * 1000), (int)(transform.position.z * 1000));
            eulerInt = new Vector3Int((int)(transform.eulerAngles.x * 1000), (int)(transform.eulerAngles.y * 1000), (int)(transform.eulerAngles.z * 1000));
            UpdateMesh();
        }
#endif

        public void SetVertices(FixedPointVector3 a, FixedPointVector3 b , FixedPointVector3 c)
        {
            vertexA = (a * 1000).ToVector3Int();
            vertexB = (b * 1000).ToVector3Int();
            vertexC = (c * 1000).ToVector3Int();
            UpdateMesh();
        }
        public void UpdateMesh()
        {
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new Mesh();
                meshFilter.sharedMesh.vertices = new Vector3[] { (Vector3)vertexA / 1000f, (Vector3)vertexB / 1000f, (Vector3)vertexC / 1000f };
                meshFilter.sharedMesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
                meshFilter.sharedMesh.triangles = new int[] { 0, 1, 2 };
                meshFilter.sharedMesh.RecalculateNormals();
            }
            else
            {
                var mesh = meshFilter.sharedMesh;
                vertices[0] = (Vector3)vertexA / 1000f;
                vertices[1] = (Vector3)vertexB / 1000f;
                vertices[2] = (Vector3)vertexC / 1000f;
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
                meshFilter.sharedMesh = mesh;
            }
        }
        private void OnDrawGizmos()
        {
            if (fixedPointTriangleCollider != null)
            {
                Gizmos.color = Color.blue;
                if (Application.isPlaying)
                {
                    Gizmos.DrawLine(fixedPointTriangleCollider.a.ToVector3(), fixedPointTriangleCollider.b.ToVector3());
                    Gizmos.DrawLine(fixedPointTriangleCollider.b.ToVector3(), fixedPointTriangleCollider.c.ToVector3());
                    Gizmos.DrawLine(fixedPointTriangleCollider.c.ToVector3(), fixedPointTriangleCollider.a.ToVector3());
                }
                else
                {
                    Gizmos.DrawLine((Vector3)vertexA / 1000, (Vector3)vertexB / 1000);
                    Gizmos.DrawLine((Vector3)vertexB / 1000, (Vector3)vertexC / 1000);
                    Gizmos.DrawLine((Vector3)vertexC / 1000, (Vector3)vertexA / 1000);
                }
                Gizmos.color = Color.white;
            }
        }
    }
}
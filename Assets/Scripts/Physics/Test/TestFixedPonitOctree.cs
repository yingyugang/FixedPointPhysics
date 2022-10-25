using BlueNoah.PhysicsEngine;
using UnityEngine;
namespace PE.Grast.Battle
{
    public class TestFixedPonitOctree : MonoBehaviour
    {
        public FixedPointOctree fixedPointOctree;
        public FixedPointSphereCollider pointTransform;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(Time.realtimeSinceStartup);
            fixedPointOctree = FixedPointOctree.Initialize(64);
            Debug.Log(Time.realtimeSinceStartup);
        }

        private void Update()
        {
            fixedPointOctree.UpdateCollider(pointTransform);
        }

        private void OnDrawGizmosSelected()
        {
            //DrawNode(node);
        }

        private void OnDrawGizmos()
        {
            if(fixedPointOctree == null || fixedPointOctree.root == null)
            {
                return;
            }
            DrawNode(fixedPointOctree.root);
        }

        void DrawNode(FixedPointOctreeNode node)
        {
            if (node.intersectedSphereColliders != null && node.intersectedSphereColliders.Count >0)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.white;
            Gizmos.DrawWireCube(node.pos.ToVector3(), Vector3.one * node.halfSize * 2);
            if (node.nodes != null)
            {
                foreach (var item in node.nodes)
                {
                    if (node.halfSize >= 4)
                    {
                        DrawNode(item);
                    }
                }
            }
        }

    }
}
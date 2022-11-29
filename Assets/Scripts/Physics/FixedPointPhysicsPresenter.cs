using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointPhysicsPresenter : SimpleSingleMonoBehaviour<FixedPointPhysicsPresenter>
    {
        public List<FixedPointRigidbody> fixedPointRigidbodies { get; private set; } = new List<FixedPointRigidbody>();
        public FixedPoint64 DeltaTime { get; set; } = 0.0333;

        public static FixedPointCollider RaycastUpDown(FixedPoint64 x, FixedPoint64 z, int layerMask = 0)
        {
            return Instance.fixedPointOctree.Raycast2DAABB(x, z, layerMask);
        }
        public static List<FixedPointCollider> RaycastAll(FixedPointVector3 start, FixedPointVector3 direct, FixedPoint64 length, int layerMask = 0)
        {
            return Instance.fixedPointOctree.RaycastAll(start, direct, length, layerMask);
        }
        public static bool Raycast(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length, out FixedPointRaycastHit fixedPointRaycastHit, int layerMask = 0)
        {
            return Instance.fixedPointOctree.Raycast(origin, direct, length, out fixedPointRaycastHit, layerMask);
        }

        public static int RaycastNonAlloc(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length, FixedPointRaycastHit[] fixedPointRaycastHits, int layerMask = 0)
        {
            return Instance.fixedPointOctree.RaycastNonAlloc(origin, direct, length, fixedPointRaycastHits, layerMask);
        }

        public static List<FixedPointCollider> OverlapSphere(FixedPointVector3 position, FixedPoint64 radius, int layerMask = -1, bool includeTrigger = false)
        {
            return Instance.fixedPointOctree.OverlapSphere(position, radius, layerMask, includeTrigger);
        }

        public static List<FixedPointCollision> OverlaySphereCollision(FixedPointVector3 position, FixedPoint64 radius, int layerMask = -1, bool includeTrigger = false)
        {
            return Instance.fixedPointOctree.OverlaySphereCollision(position, radius, layerMask, includeTrigger);
        }

        public static int OverlapSphereNonAlloc(FixedPointCollider[] colliders, FixedPointVector3 position, FixedPoint64 radius, int layerMask = 0)
        {
            return Instance.fixedPointOctree.OverlapSphereNonAlloc(colliders, position, radius, layerMask);
        }

        public static List<FixedPointCollider> OverlapAABB(FixedPointVector3 center, FixedPointVector3 size, int layerMask = 0, bool includeTrigger = false)
        {
            return Instance.fixedPointOctree.OverlapAABB(center, size, layerMask);
        }

        public static int OverlapAABBNonAlloc(FixedPointCollider[] colliders, FixedPointVector3 center, FixedPointVector3 size, int layerMask = 0)
        {
            return Instance.fixedPointOctree.OverlapAABBNonAlloc(colliders, center, size, layerMask);
        }
        public FixedPointOctree fixedPointOctree { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            fixedPointOctree = FixedPointOctree.Initialize(128);
            Debug.Log(Time.fixedDeltaTime);
            DeltaTime = Time.fixedDeltaTime;
        }

        public void OnUpdate()
        {
            fixedPointOctree.UpdateColliders();
            foreach (var item in fixedPointRigidbodies)
            {
                item.ApplyForces();
            }
            foreach (var item in fixedPointRigidbodies)
            {
                item.OnUpdate();
            }
            foreach (var item in fixedPointRigidbodies)
            {
                item.SolveConstraints();
            }
        }

        private void OnDrawGizmosSelected()
        {
            //foreach (var item in colliders)
            //{
                //Gizmos.DrawSphere(item.position.ToVector3(),item.radius.AsFloat());
            //}
            if (fixedPointOctree == null || fixedPointOctree.root == null)
            {
                return;
            }
            DrawNode(fixedPointOctree.root);
        }

        public void AddRigidbody(FixedPointRigidbody rigidbody)
        {
            fixedPointRigidbodies.Add(rigidbody); 
        }
        //TODO
        public void RemoveRigidbody(FixedPointRigidbody rigidbody)
        {

        }
        void DrawNode(FixedPointOctreeNode node)
        {
            if (node.intersectedSphereColliders.Count > 0  || node.intersectedAABBColliders.Count > 0 || node.intersectedOBBColliders.Count > 0 || node.intersectedTriangleColliders.Count > 0)
                Gizmos.color = Color.red;
            else
                Gizmos.color = new Color(1,1,1,0.2f);
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
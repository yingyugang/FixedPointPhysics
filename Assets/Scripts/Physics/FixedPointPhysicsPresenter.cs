using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointPhysicsPresenter : SimpleSingleMonoBehaviour<FixedPointPhysicsPresenter>
    {
        [HideInInspector]
        public List<FixedPointRigidbody> fixedPointRigidbodies = new List<FixedPointRigidbody>();
        [HideInInspector]
        public List<FixedPointCharacterController> actors = new List<FixedPointCharacterController>();

        public readonly static FixedPointVector3 GravitationalAcceleration = new FixedPointVector3(0, -9.82, 0);
        public void Clear()
        {
            fixedPointRigidbodies.Clear();
            actors.Clear();
            fixedPointOctree.Reset();
        }
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

        public static List<FixedPointCollision> OverlayBoxCollision(FixedPointVector3 position, FixedPointVector3 halfSize, FixedPointMatrix orientation, int layerMask = -1, bool includeTrigger = false)
        {
            return Instance.fixedPointOctree.OverlayBoxCollision(position, halfSize, orientation, layerMask, includeTrigger);
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

            #region Update the actor
            CharacterApplyForces();
            CharacterOnUpdates();
            UpdateCharacterConstraints();
            #endregion

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
        void CharacterApplyForces()
        {
            for (int i = 0; i < actors.Count; i++)
            {
                actors[i].AddForce();
            }
            
            //calculate the impluses between actors;
            for (int i = 0; i < actors.Count; i++)
            {
                for (int j = i; j < actors.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    var collision = FixedPointIntersection.IntersectWithSphereAndSphere(actors[i].fixedPointTransform.fixedPointPosition + new FixedPointVector3(0, actors[i].radius, 0), actors[i].radius, actors[j].fixedPointTransform.fixedPointPosition + new FixedPointVector3(0, actors[i].radius, 0), actors[j].radius);
                    if (collision.hit)
                    {
                        var depth1 = collision.depth * 2 * actors[i].mass / (actors[i].mass + actors[j].mass);
                        var depth2 = collision.depth * 2 * actors[j].mass / (actors[i].mass + actors[j].mass);
                        actors[i].AddImpulse(collision.normal * depth1);
                        actors[j].AddImpulse(-collision.normal * depth2);
                    }
                }
            }
        }
        void CharacterOnUpdates()
        {
            for (int i = 0; i < actors.Count; i++)
            {
                actors[i].OnUpdate();
            }
        }
        void UpdateCharacterConstraints()
        {
            //calculate the impulses between actor and static objects;
            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                var colliders = OverlaySphereCollision(actor.fixedPointTransform.fixedPointPosition + new FixedPointVector3(0, actor.radius, 0), actor.radius);
                foreach (var collision in colliders)
                {
                    if (collision.hit)
                    {
                        actor.AddConstraints(collision.normal * collision.depth * 2);
                    }
                }
                actor.fixedPointTransform.fixedPointPosition += actor.constraint;
                actor.transform.position = actor.fixedPointTransform.fixedPointPosition.ToVector3();
                var constraintNormal = actor.constraint.normalized;
                var dot = FixedPointVector3.Dot(actor.velocity, constraintNormal);
                if (dot < 0 && actor.constraint != FixedPointVector3.zero)
                {
                    actor.velocity -= constraintNormal * dot;

                    actor.velocity -= actor.velocity * actor.friction;
                }
                actor.constraint = FixedPointVector3.zero;
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
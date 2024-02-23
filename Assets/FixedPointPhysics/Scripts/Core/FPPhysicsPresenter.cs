using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    [DefaultExecutionOrder(-999)]
    internal class FPPhysicsPresenter : SimpleSingleMonoBehaviour<FPPhysicsPresenter>
    {
        internal readonly FastList<FPRigidbody> fixedPointRigidbodies = new ();
        private readonly List<FPCharacterController> fixedPointCharacterControllers = new ();
        internal readonly FastList<FPGameObject> fixedPointGameObjectFastList = new ();
        internal static readonly FixedPointVector3 GravitationalAcceleration = new (0, -9.82, 0);
        internal FPOctree fpOctree { get; private set; }
        private static readonly FixedPoint64 CharacterReboundThreshold = -4;
        public static readonly FixedPoint64 CommonDivision = 0.001;
        internal const int CommonMultiple = 1000;

        private static readonly FixedPoint64 AdditionRadius　= -FixedPoint64.EN4;
        //The interval between two physics frames.
        public FixedPoint64 DeltaTime { get; set; } = 0.0333;
        //The time physics system has been running.
        public FixedPoint64 TimeSinceStart { get; private set; }
        private int frame { get; set; }
        public bool drawGizmos = true;
        protected override void Awake()
        {
            base.Awake();
            fpOctree = FPOctree.Initialize(1024);
            DeltaTime = Time.fixedDeltaTime;
        }
        
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            Debug.Log("OnScriptsReloaded");
        }
#endif
        
        internal void Clear()
        {
            fixedPointRigidbodies.Clear();
            fixedPointCharacterControllers.Clear();
            fpOctree.Reset();
        }
        
        public void OnUpdate()
        {
            fpOctree.UpdateColliders();

            #region Update the actor
            CharacterApplyForces();
            CharacterInteractions();
            CharacterOnUpdates();
            //UpdateCharacterConstraints();
            #endregion

            #region Update the rigidbody
            for (var i = 0; i < fixedPointRigidbodies.Count; i++)
            {
                fixedPointRigidbodies[i].ApplyForces();
            }
            for (var i = 0; i < fixedPointRigidbodies.Count; i++)
            {
                fixedPointRigidbodies[i].OnUpdate();
            }
            for (var i = 0; i < fixedPointRigidbodies.Count; i++)
            {
                fixedPointRigidbodies[i].SolveConstraints();
            }
            #endregion

            for (var i = 0; i < fixedPointGameObjectFastList.Count; i++)
            {
                if (fixedPointGameObjectFastList[i] == null)
                {
                    continue;
                }
                fixedPointGameObjectFastList[i].OnLogicUpdate();
            }
            TimeSinceStart += DeltaTime;
            frame++;
        }

        public void OnViewUpdate()
        {
            for (var i = 0; i < fixedPointGameObjectFastList.Count; i++)
            {
                if (fixedPointGameObjectFastList[i] == null)
                {
                    continue;
                }
                fixedPointGameObjectFastList[i].OnViewUpdate();
            }

            foreach (var actor in fixedPointCharacterControllers)
            {
                if (actor == null)
                {
                    continue;
                }
                actor.OnViewUpdate();
            }
        }

        private void CharacterApplyForces()
        {
            foreach (var actor in fixedPointCharacterControllers)
            {
                if (actor.enabled)
                {
                    actor.AddForce();
                }
            }
        }
        
        //calculate dynamic collisions.
        private void CharacterInteractions()
        {
            //calculate the impulses between actors;
            for (var i = 0; i < fixedPointCharacterControllers.Count; i++)
            {
                if (!fixedPointCharacterControllers[i].enabled)
                {
                    continue;
                }

                //Important: Check rigidbody hit at here.
                var actor = fixedPointCharacterControllers[i];
                var count = actor.characterColliderType == CharacterCollider.Sphere ?
                           fpOctree.OverlaySphereCollision(actor.position, actor.scaledRadius, ref collisions, -1, true, true) :
                           fpOctree.OverlayAACapsuleCollision(actor.startPos, actor.endPos, actor.scaledRadius, ref collisions, -1, true, true);
                for (var j = 0; j < count; j++)
                {
                    var collision = collisions[j];
                    if (!collision.hit) continue;
                    if (collision.collider.isTrigger) continue;
                    actor.AddImpulse(collision.normal * collision.depth * 2);
                    collision.collider = actor;
                    collision.collider.onCharacterCollide?.Invoke(collision);
                }
                //　Character 間の衝突判定
                for (var j = i; j < fixedPointCharacterControllers.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (!fixedPointCharacterControllers[j].enabled)
                    {
                        continue;
                    }
                    CharacterInteraction(fixedPointCharacterControllers[i],fixedPointCharacterControllers[j]);
                }
            }
        }

        private static void CharacterInteraction(FPCharacterController fpCharacter,FPCharacterController targetFpCharacter)
        {
             FPCollision collision;
            // 違うLayerのプレヤーが衝突しない
            if (fpCharacter.layer != targetFpCharacter.layer)
            {
                collision.hit = false;
            } else {
                if (!FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(fpCharacter.min, fpCharacter.max, targetFpCharacter.min, targetFpCharacter.max))
                {
                    return;
                }
                if (fpCharacter.characterColliderType == CharacterCollider.Capsule)
                {
                    collision = targetFpCharacter.characterColliderType == CharacterCollider.Capsule ? FixedPointIntersection.IntersectWithAACapsuleAndAACapsule(fpCharacter.startPos, fpCharacter.endPos, fpCharacter.scaledRadius, targetFpCharacter.startPos, targetFpCharacter.endPos, targetFpCharacter.scaledRadius) : FixedPointIntersection.IntersectWithAACapsuleAndSphere(fpCharacter.startPos, fpCharacter.endPos, fpCharacter.scaledRadius, targetFpCharacter.position, targetFpCharacter.scaledRadius);
                }
                else
                {
                    collision = targetFpCharacter.characterColliderType == CharacterCollider.Capsule ? FixedPointIntersection.IntersectWithSphereAndAACapsule(fpCharacter.position, fpCharacter.scaledRadius, targetFpCharacter.startPos, targetFpCharacter.endPos, targetFpCharacter.scaledRadius) : FixedPointIntersection.IntersectWithSphereAndSphere(fpCharacter.position, fpCharacter.scaledRadius, targetFpCharacter.position, targetFpCharacter.scaledRadius);
                }
                
                if (!collision.hit) return;
                // 軽いの方がプッシュしやすい
                var depth1 = collision.depth * 2 * targetFpCharacter.mass / (fpCharacter.mass + targetFpCharacter.mass);
                var depth2 = collision.depth * 2 * fpCharacter.mass / (fpCharacter.mass + targetFpCharacter.mass);
                fpCharacter.AddImpulse(collision.normal * depth1);
                targetFpCharacter.AddImpulse(-collision.normal * depth2);
            }
        }

        private void CharacterOnUpdates()
        {
            foreach (var actor in fixedPointCharacterControllers)
            {
                if (actor.enabled)
                {
                    actor.OnUpdate();
                }
            }
        }

        private List<FPCollision> collisions = new ();
        public void SolveConstraints(FPCharacterController actor)
        {
            if (!actor.enabled)
            {
                return;
            }
            var count = actor.characterColliderType == CharacterCollider.Sphere ?
                            fpOctree.OverlaySphereCollision(actor.position, actor.scaledRadius, ref collisions, -1, true) :
                            fpOctree.OverlayAACapsuleCollision(actor.startPos, actor.endPos, actor.scaledRadius, ref collisions, -1, true);
            actor.isGround = false;

            for (var i = 0; i < count; i++)
            {
                var collision = collisions[i];
                if (!collision.hit) continue;
                if (!collision.collider.isTrigger)
                {
                    if (collision.closestPoint.y < actor.fpTransform.position.y + actor.scaledRadius + AdditionRadius)
                    {
                        //(Old)actor.radius * 0.29より低いなら、45degree超えるので、落とす。（１ーCos45）＝　０.２９
                        //(New)45degreeを登れる為に、50degree超えるので、落とす50degree超えるので、落とす。（１ーCos50）＝　０.36
                        if ((collision.collider.layer & (1 << LayerConstant.WallLayer)) == 0)
                        {
                            actor.isGround = true;
                            actor.groundNormal = collision.normal;
                            //FixedPointVector3.Dot(actor.preVelocity ,collision.normal) < 0 -> 登れるかどうかの判断
                            if (actor.preVelocity != FixedPointVector3.zero && FixedPointVector3.Dot(actor.preVelocity, collision.normal) < 0)
                            {
                                //登る時に落ちないようにFixedPointVector3.upを使う
                                actor.AddConstraints(FixedPointVector3.up * collision.depth * 2);
                                AdjustVelocityByCollision(actor, FixedPointVector3.up, collision.collider.rebound);
                            }
                            else
                            {
                                //他の時に落ちないようにcollision.normalを使う
                                actor.AddConstraints(collision.normal * (collision.depth * 2));
                                AdjustVelocityByCollision(actor, collision.normal, collision.collider.rebound);
                            }
                            /*
                                //Debug.Log($"{collision.closestPoint.y}||{actor.fixedPointTransform.position.y }||{actor.radius * 0.71}");
                                actor.isGround = true;
                                actor.groundPhysicsMaterial = collision.collider.physicsMaterial;
                                actor.groundNormal = collision.normal;
                                var dot1 = FixedPointVector3.Dot(collision.normal, FixedPointVector3.up);
                                actor.AddConstraints(FixedPointVector3.up * (collision.depth * 2 / dot1) * 0.9);
                                AdjustVelocityByCollision(actor, FixedPointVector3.up, collision.collider.rebound);
                                */
                        }
                        else
                        {
                            actor.AddConstraints(collision.normal * (collision.depth * 2));
                            AdjustVelocityByCollision(actor, collision.normal, collision.collider.rebound);
                        }
                    }
                    else
                    {
                        actor.AddConstraints(collision.normal * (collision.depth * 2));
                        AdjustVelocityByCollision(actor, collision.normal, collision.collider.rebound);
                    }
                }
                collision.collider = actor;
                collision.collider.onCharacterCollide?.Invoke(collision);
            }

            if (actor.isGround)
            {
                actor.fallDuration = 0;
                if (actor.colliderState == FPCharacterController.CharacterColliderState.Fall)
                {
                    actor.onLand?.Invoke();
                }
                actor.colliderState = FPCharacterController.CharacterColliderState.Ground;
            }
            else
            {
                if (!FPPhysics.Raycast(actor.fpTransform.position + new FixedPointVector3(0, actor.scaledRadius, 0), FixedPointVector3.down, actor.scaledRadius + FPCharacterController.stepHeight, out var _ , 0 ,false))
                {
                    actor.fallDuration += DeltaTime;
                    if (actor.fallDuration > 0.1 && actor.colliderState == FPCharacterController.CharacterColliderState.Ground)
                    {
                        actor.colliderState = FPCharacterController.CharacterColliderState.Fall;
                        actor.onOffGround?.Invoke();
                    }
                }
                else
                {
                    actor.fallDuration = 0;
                    if (actor.colliderState == FPCharacterController.CharacterColliderState.Fall)
                    {
                        actor.onLand?.Invoke();
                    }
                    actor.colliderState = FPCharacterController.CharacterColliderState.Ground;
                }
            }
            var constraint = actor.constraint;
            /* 他のStaticColliderとチェックする時に地面の優先度高くなって地面下に陥ちらないように。一旦保存
            if (actor.groundNormal != FixedPointVector3.zero)
            {
                var dotGroundNormal = FixedPointVector3.Dot(actor.constraint, actor.groundNormal);
                actor.constraint -= actor.groundNormal * dotGroundNormal;
            }
            */
            var normal = constraint.normalized;
            var dot = FixedPointVector3.Dot(actor.velocity.normalized, normal);
            // Velocityがconstraintと逆分量がある場合、逆分量を減少
            if ((dot <= 0 && constraint != FixedPointVector3.zero) || actor.isGround)
            {
                actor.velocity -= actor.velocity * (1 + dot) * FixedPointVector3.Dot(normal,FixedPointVector3.up);// * speed;
            }
            var speed = actor.knockBackVelocity.magnitude;
            speed = FixedPointMath.Min(speed, actor.frictionKnockBack);
            actor.knockBackVelocity -= actor.knockBackVelocity * speed;
            actor.SolveConstraints();
            actor.UpdateCollider();
        }

        /*
        private FixedPointQuaternion DeltaMoveToAngleSpeed(FixedPointVector3 delta, FixedPoint64 radius, FixedPointVector3 constraint)
        {
            var cross = FixedPointVector3.Cross(delta.normalized, constraint);
            var radian = delta.magnitude / radius * FixedPoint64.Rad2Deg;
            //注意
            //Rotate by local axis: transform.rotation =  transform.rotation * FixedPointQuaternion.AngleAxis(-radian, cross.normalized);
            //Rotate by world axis: transform.rotation = FixedPointQuaternion.AngleAxis(-radian, cross.normalized) * transform.rotation;
            return FixedPointQuaternion.AngleAxis(-radian, cross.normalized);
        }
        */

        private static void AdjustVelocityByCollision(FPCharacterController actor,FixedPointVector3 constraintNormal,FixedPoint64 rebound)
        {
            var dot = FixedPointVector3.Dot(actor.velocity, constraintNormal);
            if (dot < 0)
            {
                if (dot > CharacterReboundThreshold)
                {
                    rebound = 0;
                }
                actor.velocity = actor.velocity - constraintNormal * dot * (1 + rebound + actor.rebound);
                actor.knockBackVelocity -= FixedPointVector3.Project(actor.knockBackVelocity, constraintNormal);
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos)
            {
                return;
            }
            if (fpOctree?.root == null)
            {
                return;
            }
            DrawNode(fpOctree.root);
        }
        
        public void AddRigidbody(FPRigidbody rigidbody)
        {
            fixedPointRigidbodies.Add(rigidbody); 
        }
        
        public void AddCharacter(FPCharacterController controller)
        {
            fixedPointCharacterControllers.Add(controller);
        }

        private static void DrawNode(FPOctreeNode node)
        {
            if (node.FpSphereStack is { Count: > 0 }
                || node.FpAABBStack is { Count: > 0 }
                || node.FpObbStack is { Count: > 0 } 
                || node.FpAACapsuleStack is { Count: > 0 }
                || node.FpCapsuleStack is { Count: > 0 }
                || node.FpCylinderStack is { Count: > 0 }
                || node.FpMeshStack is { Count: > 0 }
                || node.FpCharacterStack is { Count: > 0 }
                )
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(node.pos.ToVector3(), Vector3.one * node.halfSize * 2);

            } 
            Gizmos.color = Color.white;
            if (node.nodes == null) return;
            foreach (var item in node.nodes)
            {
                DrawNode(item);
            }
        }
    }
}
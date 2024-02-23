using BlueNoah.Math.FixedPoint;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlueNoah.PhysicsEngine
{
    public class HangingPillarPresenter : FPGameObject
    {
        public int angle = 45000;
        public int speed = 1000;
        [SerializeField] private bool knockBack = true;
        [FormerlySerializedAs("fbCapsuleCollider")] [FormerlySerializedAs("capsuleCollider")] [FormerlySerializedAs("capsuleFbCollider")] [FormerlySerializedAs("capsuleColliderPresenter")] [SerializeField] private FPCapsuleCollider fpCapsuleCollider;
        private FixedPoint64 angleFixed;
        private FixedPoint64 speedFixed;
        private FixedPointQuaternion start;
        private FixedPointQuaternion end;
        private FixedPoint64 time;
        [Header("[The force cast to player when hit by beam. Min=100,Max=4000]")] [Range(100, 4000)]
        private const int Rebound = 1000;
        public Action<FPCollision> onHit;
        private const int dampKnockBack = 2000;
        protected readonly Dictionary<FPCharacterController, FixedPoint64> cachedAffectedCharacters = new ();

        protected override void Init()
        {
            angleFixed = angle * 0.001;
            speedFixed = speed * 0.001;
            if (FPPhysicsPresenter.Instance != null)
            {
                FPPhysicsPresenter.Instance.fixedPointGameObjectFastList.Add(this);
            }
            start = FixedPointQuaternion.AngleAxis(angleFixed, fpTransform.right) *  fpTransform.rotation;
            end = FixedPointQuaternion.AngleAxis(-angleFixed, fpTransform.right) * fpTransform.rotation;
            if (fpCapsuleCollider == null) return;
            //capsuleCollider.fixedPointTransform.SetParent(fixedPointTransform);
            if (knockBack)
            {
                fpCapsuleCollider.onCharacterCollide += (collision) =>
                {
                    var character = (FPCharacterController)collision.collider;
                    if (VerifyCollisionInterval(character))
                    {
                        character.KnockBack((collision.normal) * Rebound);
                        character.dampKnockBackDamp = dampKnockBack * 0.001;
                        onHit?.Invoke(collision);
                    }
                };
            }
        }
        //go with this function to make ensure character only be hit once per time period.
        protected bool VerifyCollisionInterval(FPCharacterController fpCharacterController)
        {
            if (cachedAffectedCharacters.TryGetValue(fpCharacterController, out var character))
            {
                if (character > FPPhysicsPresenter.Instance.TimeSinceStart)
                {
                    return false;
                }
            }
            cachedAffectedCharacters[fpCharacterController] = FPPhysicsPresenter.Instance.TimeSinceStart + 0.1;
            return true;
        }

        public override void OnLogicUpdate()
        {
            time += FPPhysicsPresenter.Instance.DeltaTime;
            var t = (FixedPointMath.Cos(time * speedFixed) + 1) * 0.5;
            fpTransform.rotation = FixedPointQuaternion.Lerp(start, end,t);
        }

        public override void OnViewUpdate()
        {
        }
    }
}
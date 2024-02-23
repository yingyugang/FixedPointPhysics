/*
* Create 2023/10/5
* 応彧剛　yingyugang@gmail.com
* It's used by fixed point physics system.
* Rotate gimmicks which could hit player and knockBack player.
* Like RotateBeam or RotateHammer/Beam in game.
*/

using System;
using UnityEngine.Serialization;

namespace BlueNoah.PhysicsEngine
{
    using Math.FixedPoint;
    using System.Collections.Generic;
    using UnityEngine;
    public sealed class RotateBeamPresenter : FPGameObject
    {
        [FormerlySerializedAs("beamFbColliderRoot")] [FormerlySerializedAs("beamColliderRoot")] [SerializeField] private FPCollider beamFpColliderRoot;
        [FormerlySerializedAs("baseFbColliderRoot")] [FormerlySerializedAs("baseColliderRoot")] [SerializeField] private FPCollider baseFpColliderRoot;
        [SerializeField] private Transform beamModelRoot;
        [SerializeField]
        [Range(-500000, 500000)]
        [Header("[Rotate beam rotate speed.Min=-500000,Max=500000,100000 means 1degree/s]")]
        private int speed = 200000;
        [SerializeField]
        [Header("[The force cast to player when hit by beam. Min=100,Max=4000]")]
        [Range(100, 4000)]
        private int Rebound = 1000;
        private const int dampKnockBack = 2000;
        [SerializeField] private bool isSpinAround = true;
        private FixedPointVector3 axis;
        private FixedPointQuaternion quaternion;
        // Hit character ,use to play any sounds or effects.
        public Action<FPCollision> onHit;
        private readonly Dictionary<FPCharacterController, FixedPoint64> cachedAffectedCharacters = new ();
        //go with this function to make sure character only be hit once per time period.
        private bool VerifyCollisionInterval(FPCharacterController fpCharacterController)
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

        protected override void Init()
        {
            axis = fpTransform.up;
            // Rotate around parent transform.
            var rotatedPosition = fpTransform.rotation * beamFpColliderRoot.fpTransform.position;
            beamFpColliderRoot.fpTransform.position = fpTransform.position + rotatedPosition;
            beamFpColliderRoot.fpTransform.rotation = fpTransform.rotation * beamFpColliderRoot.fpTransform.rotation;
            // Rotate around parent transform.
            rotatedPosition = fpTransform.rotation * baseFpColliderRoot.fpTransform.position;
            baseFpColliderRoot.fpTransform.position = fpTransform.position + rotatedPosition;
            baseFpColliderRoot.fpTransform.rotation = fpTransform.rotation * baseFpColliderRoot.fpTransform.rotation;
            quaternion = FixedPointQuaternion.AngleAxis(speed * FPPhysicsPresenter.CommonDivision * FPPhysicsPresenter.Instance.DeltaTime, axis);
            beamFpColliderRoot.onCharacterCollide += (collision) =>
            {
                var character = (FPCharacterController)collision.collider;
                if (VerifyCollisionInterval(character))
                {
                    character.KnockBack((collision.normal) * Rebound);
                    character.dampKnockBackDamp = dampKnockBack * FPPhysicsPresenter.CommonDivision;
                    onHit?.Invoke(collision);
                }
            };
        }

        public override void OnLogicUpdate()
        {
            if (isSpinAround)
            {
                fpTransform.rotation = quaternion * fpTransform.rotation;
            }
        }

        public override void OnViewUpdate()
        {
            beamModelRoot.rotation = fpTransform.rotation.ToQuaternion();
        }
    }
}
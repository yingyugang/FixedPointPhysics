using BlueNoah.Math.FixedPoint;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public sealed class CannonPresenter : FPGameObject
    {
        private enum Speed { Slow, Normal, Fast}
        private enum BallSize { S, M, L }
        private enum Timer { Sec_3, Sec_4, Sec_5 }
        [Header("Shoot point.")]
        public uint shootOffset = 2;

        [SerializeField] private Timer shootTimer = Timer.Sec_3;
        [SerializeField] private BallSize ballSize = BallSize.S;
        [SerializeField] private Speed shootSpeed = Speed.Slow;
        [SerializeField] private FPRigidbodyPresenter ball_S;
        [SerializeField] private FPRigidbodyPresenter ball_M;
        [SerializeField] private FPRigidbodyPresenter ball_L;

        public Action<Transform> onBallCreated;

        [Header("[The force cast to player when hit by beam. Min=100,Max=4000]")]
        [Range(100, 4000)]
        [SerializeField]
        private int Rebound = 1000;
        private const int dampKnockBack = 2000;
        private readonly Dictionary<FPCharacterController, FixedPoint64> cachedAffectedCharacters = new ();
        private uint timer = 2500;
        private uint power = 20;
        private FPRigidbodyPresenter ball;

        //go with this function to make ensure character only be hit once per time period.
        private bool VerifyCollisionInterval(FPCharacterController fpCharacterController)
        {
            if (cachedAffectedCharacters.TryGetValue(fpCharacterController, out var character))
            {
                if (character > FPPhysicsPresenter.Instance.TimeSinceStart)
                {
                    return false;
                }
            }
            cachedAffectedCharacters[fpCharacterController] = FPPhysicsPresenter.Instance.TimeSinceStart + 1;
            return true;
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                Init();
            }
        }

        protected override void Init()
        {
            switch (shootTimer)
            {
                case Timer.Sec_3:
                    timer = 3000;
                    break;
                case Timer.Sec_4:
                    timer = 4000;
                    break;
                case Timer.Sec_5:
                    timer = 5000;
                    break;
            }
            switch (ballSize)
            {
                case BallSize.S:
                    ball = ball_S;
                    break;
                case BallSize.M:
                    ball = ball_M;
                    break;
                case BallSize.L:
                    ball = ball_L;
                    break;
            }
            switch (shootSpeed)
            {
                case Speed.Slow:
                    power = 10;
                    break;
                case Speed.Normal:
                    power = 20;
                    break;
                case Speed.Fast:
                    power = 40;
                    break;
            }
            var fixedPointTimer = new FPTimer(timer, FPPhysicsPresenter.Instance.DeltaTime, OnActive);
            if (Application.isPlaying)
            {
                FPPhysicsPresenter.Instance.fixedPointGameObjectFastList.Add(this);
            }
            fixedPointTimers.Add(fixedPointTimer);
        }

        private void OnActive(FPTimer fpTimer)
        {
            var bullet = Instantiate(this.ball, FPPhysicsPresenter.Instance.transform);
            onBallCreated?.Invoke(bullet.transform);
            bullet.fpRigidbody.transform.position = fpTransform.position + fpTransform.forward * shootOffset;
            bullet.fpRigidbody.AddLinearImpulse(fpTransform.forward * power);
            var fixedPointCollider = bullet.GetComponent<FPSphereCollider>();
            fixedPointCollider.onCharacterCollide += (collision) =>
            {
                var character = (FPCharacterController)collision.collider;
                if (!VerifyCollisionInterval(character)) return;
                character.KnockBack((collision.normal) * Rebound);
                character.dampKnockBackDamp = dampKnockBack * 0.001;
            };
            var newFixedPointTimer = new FPTimer(timer, FPPhysicsPresenter.Instance.DeltaTime, OnActive);
            fixedPointTimers.Add(newFixedPointTimer);
            newFixedPointTimer = new FPTimer(10000, FPPhysicsPresenter.Instance.DeltaTime, OnBallDestroy, bullet.gameObject);
            fixedPointTimers.Add(newFixedPointTimer);
        }

        private static void OnBallDestroy(FPTimer fpTimer)
        {
            Destroy(fpTimer.gameObject);
        }

        public override void OnLogicUpdate()
        {
            if (fixedPointTimers == null) return;
            for (var i = 0; i < fixedPointTimers.Count; i++)
            {
                fixedPointTimers[i].OnUpdate();
                if (!fixedPointTimers[i].disposed) continue;
                fixedPointTimers.Remove(fixedPointTimers[i]);
                i--;
            }
        }

        public override void OnViewUpdate()
        {
        }
    }
}
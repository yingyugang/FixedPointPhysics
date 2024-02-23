using System.Collections.Generic;
using BlueNoah.Math.FixedPoint;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlueNoah.PhysicsEngine
{
    public sealed class MovingPlatformPresenter : FPGameObject
    {
        private enum Direction {Horizontal,Vertical}
        [SerializeField]
        [Header("[MovingPlatform must be square]")]
        private Direction direction = Direction.Horizontal;
        [Header("[Min=500,Max=2500,1000 means 1m/s]")]
        [Range(500, 2500)]
        public int speed = 1000;
        [Header("[Min=2000,Max=10000,1000 means 1m]")]
        [Range(2000, 10000)]
        public int distance = 4000;
        [FormerlySerializedAs("movingCollider")] [SerializeField] private FPAABBCollider movingFbCollider;
        [FormerlySerializedAs("blockCollider")] [SerializeField] private FPAABBCollider blockFbCollider;

        private FixedPoint64 moved;
        private FixedPoint64 speedFixed;
        private FixedPointVector3 direct;
        private FixedPoint64 scopeFixed;
        private Transform blockTransform;
        private readonly List<FPCollider> impactedCharacterList = new (100);
        private int count;
        private void Awake()
        {
            Init();
        }

        protected override void Init()
        {
            if (Application.isPlaying)
            {
                FPPhysicsPresenter.Instance.fixedPointGameObjectFastList.Add(this);
            }
            speed = Mathf.Clamp(speed, 500, 2500);
            distance = Mathf.Clamp(distance, 2000, 10000);
            blockFbCollider.onCharacterCollide += OnCollision;
            if (movingFbCollider != null)
            {
                switch (direction)
                {
                    case Direction.Horizontal:
                        direct = movingFbCollider.fpTransform.rotation * FixedPointVector3.right;
                        break;
                    case Direction.Vertical:
                        direct = movingFbCollider.fpTransform.rotation * FixedPointVector3.up;
                        break;
                }
            }
            speedFixed = 0;
            scopeFixed = distance * 0.001f;
            blockTransform = blockFbCollider.transform;
            //slowdownDistance = speed * 0.001f * t + a * t * t;
            a = (slowdownDistance - Mathf.Abs(speed * 0.001f)) / (t * t);
        }

        private readonly FixedPoint64 slowdownDistance = 0.2;
        private FixedPoint64 a = 10;
        private readonly FixedPoint64 t = 0.2;
        private void OnCollision(FPCollision collision)
        {
            //　側面のキャラ移動しない
            var dot = FixedPointVector3.Dot(collision.normal,collision.collider.fpTransform.up);
            if (dot > 0.99)
            {
                if (count < impactedCharacterList.Count)
                {
                    impactedCharacterList[count] = collision.collider;
                }
                else
                {
                    impactedCharacterList.Add(collision.collider);
                }
                count++;
            }
        }
        public override void OnLogicUpdate()
        {
            if (moved > scopeFixed - slowdownDistance)
            {
                speedFixed += a * FPPhysicsPresenter.Instance.DeltaTime;
            }
            else if (moved < slowdownDistance)
            {
                speedFixed -= a * FPPhysicsPresenter.Instance.DeltaTime;
            }

            var deltaMoveVector3 = direct * speedFixed * FPPhysicsPresenter.Instance.DeltaTime;
            fpTransform.position += deltaMoveVector3;
            moved += speedFixed * FPPhysicsPresenter.Instance.DeltaTime;
            if(count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var character = (FPCharacterController)impactedCharacterList[i];
                    character.AddImpulse(deltaMoveVector3);
                }
                count = 0;
            }
        }

        public override void OnViewUpdate()
        {
            blockTransform.position = blockFbCollider.fpTransform.position.ToVector3();
        }

        private void OnDrawGizmos()
        {
            if (!enabled)
            {
                return;
            }
            var color = Gizmos.color;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position,0.3f);
            var moveDirect = FixedPointVector3.zero;
            switch (direction)
            {
                case Direction.Horizontal:
                    moveDirect= FixedPointVector3.right;
                    break;
                case Direction.Vertical:
                    moveDirect= FixedPointVector3.up;
                    break;
            }

            var trans = transform;
            var position = trans.position;
            var rotation = trans.rotation;
            Gizmos.DrawLine(position, position + rotation * (moveDirect * distance * 0.001f).ToVector3());
            Gizmos.DrawSphere(position + rotation * (moveDirect * distance * 0.001f).ToVector3(), 0.3f);
            Gizmos.color = color;
        }

        public void SetParameters(int speed, int distance)
        {
            this.speed = speed;
            this.distance = distance;
        }
    }
}
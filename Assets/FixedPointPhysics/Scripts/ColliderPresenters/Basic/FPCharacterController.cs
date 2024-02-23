using BlueNoah.Math.FixedPoint;
using System;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public enum CharacterCollider { Sphere, Capsule }
    [RequireComponent(typeof(FPTransform))]
    public sealed class FPCharacterController : FPAACapsuleCollider
    {
        public CharacterCollider characterColliderType = CharacterCollider.Capsule;
        public override ColliderType colliderType => ColliderType.CharacterController;
        /// <summary>
        /// Updates the Axis-Aligned Bounding Box based on the capsule's current dimensions and position.
        /// </summary>
        internal override void UpdateAABB()
        {
            var width = scaledRadius;
            var halfSize = new FixedPointVector3(width, characterColliderType == CharacterCollider.Sphere? width : scaledHalfHeight, width);
            _min = position - halfSize;
            _max = position + halfSize;
        }
                
        /// <summary>
        /// Removes this collider from all impact nodes it is part of, ensuring it no longer participates in collision checks.
        /// </summary>
        protected override void RemoveFromImpactNotes()
        {
            targetNode?.FpCharacterStack.Remove(this);
            targetNode = null;
        }
        
        /// <summary>
        /// Adds this collider to the specified octree node's list of capsule colliders, enabling collision checks.
        /// </summary>
        protected override void AddToImpactNote(FPOctreeNode node)
        {
            node.FpCharacterStack ??= new FPColliderStack<FPCharacterController>(node);
            node.FpCharacterStack.Add(this);
            targetNode = node;
        }

        protected override void OnDrawGizmosEditor()
        {
            if (characterColliderType == CharacterCollider.Sphere)
            {
                Gizmos.DrawWireSphere(position .ToVector3(), scaledRadius.AsFloat());
            }
            else
            {
                FPCapsuleCollider.DrawWireCapsule(startPos.ToVector3(), endPos.ToVector3(), scaledRadius.AsFloat());
            }
        }
        
        public enum CharacterColliderState { Ground, Fall }
        public int targetLayerMask { get; set; } 
        //The maximum
        public FPOctreeNode[] impactNodeArray { get; set; } = new FPOctreeNode[8];
        public int impactNodeIndex { get; set; } = -1;
        public CharacterColliderState colliderState { get; set; }= CharacterColliderState.Ground;
       
        private FixedPointVector3 impulse { get; set; }
        public FixedPointVector3 constraint { get; private set; }
        private FixedPointVector3 preConstraint { get; set; }
        public FixedPoint64 mass { 
            get => _mass;
            set {
                _mass = value;
                reverseMass = 1 / _mass;
            } 
        }

        private FixedPoint64 _mass = 1;
        public FixedPoint64 reverseMass { get;private set; } = 1;
        // 基本のキャラMass
        private static readonly FixedPoint64 BaseMass = 1;

        public void SetInteractiveFriction(FixedPoint64 friction)
        {
            friction = FixedPointMath.Max(0, friction);
            interactiveFriction = friction * FPPhysicsPresenter.Instance.DeltaTime;
        }
        public FixedPoint64 interactiveFriction { get; private set; } = 0.033;
        public void SetInteractiveAccelerate(FixedPoint64 accelerate)
        {
            accelerate = FixedPointMath.Max(0, accelerate);
            interactiveAccelerate = accelerate * FPPhysicsPresenter.Instance.DeltaTime;
        }
        public FixedPoint64 interactiveAccelerate { get; private set; } = 0.033;
        public FixedPoint64 frictionKnockBack { get; set; } = 0.1;
        public FixedPoint64 dampKnockBackDamp { get; set; } = 1;
        public static FixedPoint64 stepHeight => 1;
        public FixedPoint64 fallDuration { get; set; }
        public bool isGround{ get; set; }
        public FixedPointVector3 groundNormal{ get; set; }

        private FixedPointVector3 _velocity;
        public FixedPointVector3 velocity {
            get => _velocity;
            set =>
                // 最大速度を制限して、FixedPointの最大範囲に超えないように。ArgumentOutOfRangeException、 Negative value passed to Sqrt　というエラーを解消。
                _velocity = new FixedPointVector3(FixedPointMath.Clamp(value.x, -5000, 5000),
                    FixedPointMath.Clamp(value.y, -5000, 5000),
                    FixedPointMath.Clamp(value.z, -5000, 5000));
        }
        private FixedPointVector3 currentForce { get; set; }
        public FixedPointVector3 forces { get; set; }
        public FixedPointVector3 deltaPosition{ get; set; }
        public Action onLand;
        public Action onOffGround;
        public Action onJump;
        public Action<FixedPointVector3> onMove;
        public FixedPointVector3 preVelocity{ get; set; }
        private const int diveForce = 200;
        public int doubleJumpOffsetY { get; set; } = 100;
        private int doubleJumpCooldown{ get; set; }
        public FixedPoint64 lastJump{ get; set; }
        public FixedPointVector3 jumpForce{ get; set; }
        //Just affect when character in knockBack status.
        public FixedPointVector3 knockBackVelocity{ get; set; }
        //The force that apply to character when knockBack by something.
        public FixedPointVector3 knockBackForce{ get; set; }

        public Action<FixedPointVector3> onKnockBack;
        // １フレームでこの移動距離を超えると複回数StaticColliderに対してチェックが必要、（最低ブロック厚さが0.4m）、数値を大きくするとチェック回数が減らす。性能にの影響が大きので、小さいすぎないように。
        private static readonly FixedPoint64 ThresholdDeltaMove = 0.2;

        private static readonly FixedPoint64 JumpCooldown = 1;
        // 適当に増減できる
        private static readonly FixedPoint64 MaxAdditionGravity = 2;

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying && FPPhysicsPresenter.Instance != null)
            {
                FPPhysicsPresenter.Instance.AddCharacter(this);
            }
        }

        public void Reset()
        {
            knockBackVelocity = FixedPointVector3.zero;
            knockBackForce = FixedPointVector3.zero;
            velocity = FixedPointVector3.zero;
            currentForce = FixedPointVector3.zero;
            forces = FixedPointVector3.zero;
        }
        public void Jump(FixedPoint64 jumpHigh)
        {
            lastJump = JumpCooldown;
            var t = FixedPointMath.Sqrt(FixedPointMath.Abs(jumpHigh * reverseMass / FPPhysicsPresenter.GravitationalAcceleration.y * 2));
            var v = t * FPPhysicsPresenter.GravitationalAcceleration.y;
            jumpForce = new FixedPointVector3(0, -(v / FPPhysicsPresenter.Instance.DeltaTime).AsFloat(), 0);
            AddForce(jumpForce);
            onJump?.Invoke();
        }

        public bool JumpAble()
        {
            return lastJump < 0;
        }

        public void Move(FixedPointVector3 velocity)
        {
            if (isGround)
            {
                MoveWithAccelerateFrictionOnGround(velocity);
                /*
                return;
                #region 人間視力があるから、斜面の時に自ら斜面の合わせて方向調整できる。
                var dot = FixedPointVector3.Dot(velocity, groundNormal);
                {
                    var speed = velocity.magnitude;
                    var newVelocity = velocity - groundNormal * dot;
                    velocity = newVelocity.normalized * speed;
                }
                #endregion
                if (groundPhysicsMaterial == FixedPointPhysicsMaterial.Ice)
                {
                    MoveWithAccelerateFrictionOnGround(velocity);
                }
                else
                {
                    var speed = preVelocity.magnitude;
                    var speed1 = velocity.magnitude;
                    var direct = velocity.normalized;
                    var direct1 = preVelocity.normalized;
                    if (speed1 < 0.01)
                    {
                        direct = direct1;
                    }
                    //反対方向に転身したら、元々のSpeedをクリア。
                    dot = FixedPointVector3.Dot(direct, direct1);
                    if (dot < -0.5)
                    {
                        //speed = 0;
                        //speed1 = 0;
                    }
                    //preVelocity = FixedPointMath.Lerp(speed, speed1, velocityFactor) * direct;
                    var right = FixedPointVector3.Cross(direct, groundNormal);
                    var forward = FixedPointVector3.Cross(groundNormal, right);
                    var newSpeed = speed1;
                    if (speed < speed1)
                    {
                        newSpeed = FixedPointMath.Lerp(speed, speed1, velocityFactor);
                    }
                    var newVelocity = newSpeed * forward;
                    AddImpulse(preVelocity);
                    preVelocity = newVelocity;
                }*/
            }
            else
            {
                MoveWithAccelerateFrictionOffGround(velocity);
                /*
                if (groundPhysicsMaterial == FixedPointPhysicsMaterial.Ice)
                {
                    //Stimulate slipping.
                    var speed = preVelocity.magnitude;
                    var direct = preVelocity.normalized;
                    speed = FixedPointMath.Max(0, speed - interactiveFriction * FixedPointPhysicsPresenter.Instance.DeltaTime);
                    var targetSpeed = velocity.magnitude;
                    var targetDirect = velocity.normalized;
                    var newVelocity = direct * speed + targetDirect * FixedPointMath.Max(0, interactiveFriction * 2 * FixedPointPhysicsPresenter.Instance.DeltaTime);
                    newVelocity = FixedPointMath.Min(maxSpeed * FixedPointPhysicsPresenter.Instance.DeltaTime, newVelocity.magnitude) * newVelocity.normalized;
                    AddImpulse(preVelocity * jumpSpeedProportion);
                    preVelocity = newVelocity;
                }
                else
                {
                    AddImpulse(preVelocity * jumpSpeedProportion);
                    preVelocity = FixedPointVector3.Lerp(preVelocity, velocity, inertiaFactor);
                }*/
            }
            //currentForce += velocity * 100;
        }

        //Related to MoveWithAccelerateFriction, it is more real like human.
        /*
        void MoveWithAccelerateFrictionOnly(FixedPointVector3 velocity, FixedPoint64 dot)
        {
            //Stimulate slipping.
            var speed = preVelocity.magnitude;
            var direct = preVelocity.normalized;
            speed = FixedPointMath.Max(0, speed - interactiveFriction * FixedPointPhysicsPresenter.Instance.DeltaTime);
            //var targetSpeed = velocity.magnitude;
            var targetDirect = velocity.normalized;
            //TODO　複数のGroundで場合、groundが合成するように
            //Gravityを模擬
            var deltaGravityVelocity = FixedPointPhysicsPresenter.Instance.DeltaTime * FixedPointPhysicsPresenter.GravitationalAcceleration;
            dot = FixedPointVector3.Dot(deltaGravityVelocity, groundNormal);
            if (dot < 0)
            {
                deltaGravityVelocity = deltaGravityVelocity - groundNormal * dot;
            }
            //人間の走るによってGravityを相殺するを模擬(普通のRigidbodyと違う点、人間が両足で走るから)
            dot = FixedPointVector3.Dot(deltaGravityVelocity, targetDirect);
            if (dot < 0)
            {
                deltaGravityVelocity = deltaGravityVelocity - targetDirect * dot;
            }
            //キャラの内部動力によって速度の変更
            var internalPropulsion = targetDirect * FixedPointMath.Max(0, interactiveFriction * 2 * FixedPointPhysicsPresenter.Instance.DeltaTime);

            var newVelocity = direct * speed + deltaGravityVelocity * FixedPointPhysicsPresenter.Instance.DeltaTime + internalPropulsion;
            newVelocity = FixedPointMath.Min(maxSpeed * FixedPointPhysicsPresenter.Instance.DeltaTime, newVelocity.magnitude) * newVelocity.normalized;
            AddImpulse(newVelocity);
            preVelocity = newVelocity;
        }*/

        private void MoveWithAccelerateFrictionOnGround(FixedPointVector3 targetVelocity)
        {
            #region 方向を地面の角度によって方向修正する。(斜面で操作速度を落ちないように)
            var targetSpeed = targetVelocity.magnitude;
            var dotGround = FixedPointVector3.Dot(targetVelocity, groundNormal);
            var targetDirect = (targetVelocity - groundNormal * dotGround).normalized;
            #endregion
            MoveWithAccelerateFriction(targetDirect,targetSpeed);
        }

        private void MoveWithAccelerateFrictionOffGround(FixedPointVector3 targetVelocity)
        {
            var targetSpeed = targetVelocity.magnitude;
            var targetDirect = targetVelocity.normalized;
            MoveWithAccelerateFriction(targetDirect, targetSpeed);
        }

        /// <summary>
        /// FrictionとAccelerationによって移動
        /// Related to MoveWithAccelerateFriction, it is more real like in game.
        /// FrictionとAccelerateがあるけど、MaxSpeedもある。
        /// FrictionとAccelerateがマスターデーターで調整しやすいため、Line的に速度に影響するように調整。
        /// Accelerateした後に速度がMaxSpeedによって再調整。
        /// </summary>
        private void MoveWithAccelerateFriction(FixedPointVector3 targetDirect, FixedPoint64 targetSpeed)
        {
            #region 方向上に静態Colliderがある場合、静態Colliderに入らないように、この移動をスキップ。
            var correctVelocity = targetDirect * targetSpeed;
            if (preConstraint != FixedPointVector3.zero)
            {
                var preConstraintDirect = preConstraint.normalized;
                var dot = FixedPointVector3.Dot(correctVelocity, preConstraintDirect);
                if (dot < 0)
                {
                    return;
                }
            }
            #endregion

            #region 既存速度のtargetDirect上の速度分量
            var preTargetDirectSpeed = FixedPointVector3.Dot(preVelocity, targetDirect);
            var directVelocity = targetDirect * preTargetDirectSpeed;
            var lateralVelocity = preVelocity - directVelocity;
            var invMass = reverseMass * FPPhysicsPresenter.Instance.DeltaTime;
            if (preTargetDirectSpeed < targetSpeed)
            {
                var deltaNewSpeed = FixedPointMath.Min(targetSpeed - preTargetDirectSpeed, interactiveAccelerate * invMass);
                directVelocity += targetDirect * deltaNewSpeed;
                if (preTargetDirectSpeed < 0)
                {
                    directVelocity += targetDirect * interactiveFriction * invMass;
                }
            }
            else if (preTargetDirectSpeed > targetSpeed)
            {
                var deltaNewSpeed = FixedPointMath.Min(preTargetDirectSpeed - targetSpeed, interactiveFriction * invMass);
                directVelocity -= targetDirect * deltaNewSpeed;
            }
            #endregion
            
            #region 今キャラのUser操作方向の横（90度垂直方向上）の速度分量、Frictionによってこの速度を減少
            var deltaFrictionSpeed = interactiveFriction * invMass;
            lateralVelocity = lateralVelocity.normalized * FixedPointMath.Max(0, lateralVelocity.magnitude - deltaFrictionSpeed);
            #endregion
            
            var newVelocity = lateralVelocity + directVelocity;
            AddImpulse(newVelocity);
            preVelocity = newVelocity;
        }

        public void Dive(FixedPointVector3 orientation)
        {
            AddForce((orientation + new FixedPointVector3(0, doubleJumpOffsetY / 1000f, 0)) * diveForce);
            velocity = FixedPointVector3.zero;
            doubleJumpCooldown = 60;
        }
        //The force accept from internal.
        public void AddForce(FixedPointVector3 force)
        {
            //currentForce += force;
            currentForce = force;
        }

        //The force accept from external.
        public void KnockBack(FixedPointVector3 force)
        {
            preVelocity = FixedPointVector3.zero;
            knockBackForce = force;
            velocity = FixedPointVector3.zero;
            onKnockBack?.Invoke(force);
        }
        
        public void AddForce()
        {
            if (isGround)
            {
                forces = currentForce;
            }
            else
            {
                // new FixedPointVector3(0, mass - BaseMass, 0), Mass が大きくほど加速度がたかくなる。１mass増やする加速度が１ｍ/sを増やす
                forces = currentForce + FPPhysicsPresenter.GravitationalAcceleration - new FixedPointVector3(0, FixedPointMath.Min(MaxAdditionGravity, mass - BaseMass) , 0) ;
            }
            knockBackVelocity += knockBackForce * FPPhysicsPresenter.Instance.DeltaTime;
            velocity += forces * FPPhysicsPresenter.Instance.DeltaTime;
            currentForce = FixedPointVector3.zero;
            knockBackForce = FixedPointVector3.zero;
        }
        
        public void AddImpulse(FixedPointVector3 additionalImpulse)
        {
            if (impulse == FixedPointVector3.zero)
            {
                impulse = additionalImpulse;
            }
            else
            {
                var impulseNormal = impulse.normalized;
                var magnitude = impulse.magnitude;
                var dot = FixedPointVector3.Dot(additionalImpulse, impulseNormal);
                if (dot > magnitude)
                {
                    impulse = additionalImpulse;
                }
                else if (dot > 0)
                {
                    impulse += additionalImpulse - impulseNormal * dot;
                }
                else
                {
                    impulse += additionalImpulse;
                }
            }
        }
        internal void AddConstraints(FixedPointVector3 additionalConstraint)
        {
            if (constraint == FixedPointVector3.zero)
            {
                constraint = additionalConstraint;
            }
            else
            {
                var constraintNormal = constraint.normalized;
                var magnitude = constraint.magnitude;
                var dot = FixedPointVector3.Dot(additionalConstraint, constraintNormal);
                // additionalConstraintが既存のconstraint方向の分量が同じ方向、尚且つconstraintより大き。
                if (dot > magnitude)
                {
                    constraint = additionalConstraint;
                }
                // additionalConstraintが既存のconstraint方向の分量が同じ方向、尚且つconstraintより小さい。
                else if (dot > 0)
                {
                    constraint += additionalConstraint - constraintNormal * dot;
                }
                else
                {
                    // additionalConstraintが既存のconstraint方向の分量が逆の場合。
                    constraint += additionalConstraint;
                }
            }
        }
        
        // すべでのStaticColliderの衝突判定が終わったら、Characterのポジション修正
        internal void SolveConstraints()
        {
            fpTransform.position += constraint;
            preConstraint = constraint;
            constraint = FixedPointVector3.zero;
        }
        
        internal void ChangeVelocityDirection(FixedPointVector2 forward)
        {
            var y = velocity.y;
            var magnitude = new FixedPointVector2(velocity.x, velocity.z).magnitude;
            velocity = new FixedPointVector3(forward.x * magnitude, y, forward.y * magnitude);
        }
        
        internal void OnUpdate()
        {
            if (!enabled)
            {
                return;
            }
            lastJump -= FPPhysicsPresenter.Instance.DeltaTime;
            // velocity が地面の時にZeroに設定、上に斜面があるときにジャンプすると滑りがあるという問題の解決ために。
            if (isGround && velocity != FixedPointVector3.zero)
            {
                if (FixedPointVector3.Dot(velocity, groundNormal) <= FixedPoint64.EN3)
                {
                    velocity = FixedPointVector3.zero;
                }
            }
            deltaPosition = impulse + (velocity + knockBackVelocity) * FPPhysicsPresenter.Instance.DeltaTime;
            
            /*
            // 方向上に静態Colliderがある場合、静態Colliderに入らないように、この移動をスキップ。
            if (preConstraint != FixedPointVector3.zero)
            {
                var preConstraintDirect = preConstraint.normalized;
                var dot = FixedPointVector3.Dot(deltaPosition, preConstraintDirect);
                if (dot < 0)
                {
                    deltaPosition = FixedPointVector3.zero;
                }
            }*/
            
            impulse = FixedPointVector3.zero;
            if (doubleJumpCooldown > 0)
            {
                doubleJumpCooldown--;
            }
            var magnitude = deltaPosition.magnitude;
            if (magnitude > ThresholdDeltaMove)
            {
                var count = (int)(magnitude / ThresholdDeltaMove) + 1;
                var reverse = 1 / new FixedPoint64(count);
                for (var i = 0; i < count; i++)
                {
                    fpTransform.position += deltaPosition * reverse;
                    FPPhysicsPresenter.Instance.SolveConstraints(this);
                }
                return;
            }

            fpTransform.position += deltaPosition;
            FPPhysicsPresenter.Instance.SolveConstraints(this);
        }

        internal void OnViewUpdate()
        {
            transform.position = fpTransform.position.ToVector3();
        }
    }
}
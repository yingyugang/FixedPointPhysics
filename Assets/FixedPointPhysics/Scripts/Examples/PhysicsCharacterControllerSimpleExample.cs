using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public enum CharacterState { Normal, KnockBack, Jump , Dive, DiveGetUp, KnockBackGetUp, Victory, DoubleJump }
    public class PhysicsCharacterControllerSimpleExample : SimpleSingleMonoBehaviour<PhysicsCharacterControllerSimpleExample>
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private FPCharacterController capsuleActor;
        [SerializeField] private FPCharacterController sphereActor;
        public FPCharacterController actor { get; private set; }
        [SerializeField] private SimpleRpgCamera cameraController;
        
        private FixedPoint64 moveSpeed = 3;
        private readonly FixedPoint64 diveGetUpTime = 0.2;
        private FixedPoint64 diveGetUpDuration = 0;
        private readonly FixedPoint64 knockBackGetUpTime = 1;
        private FixedPoint64 knockBackUpDuration = 0;
        private FixedPoint64 fallDuration;
        private FixedPoint64 diveDuration;
        private readonly FixedPoint64 diveTime = 1;
        private static readonly Quaternion zeroQ = new Quaternion(0, 0, 0, 0);
        private CharacterState characterState = CharacterState.Normal;
        private FixedPoint64 knockBackCooldown;
        private FixedPointVector3 startPosition;
        private FixedPoint64 jumpHigh;
        private FixedPoint64 diveSpeed;
        public float mass = 1;
        public int characterAccelerationFactor = 100000;
        public int interactiveFriction = 100000;
        private Vector3 preTransformPos;
        private FixedPointVector3 movement;
        
        protected override void Awake()
        {
            base.Awake();
            InitActors();
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            actor = capsuleActor;
        }

        private void Start()
        {
            startPosition = actor.fpTransform.position;
        }

        public void SwitchToCapsule()
        {
            actor = capsuleActor;
            cameraController.target = actor.transform;
        }

        public void SwitchToSphere()
        {
            actor = sphereActor;
            cameraController.target = actor.transform;
        }

        private void InitActors()
        {
            InitActor(capsuleActor);
            InitActor(sphereActor);
        }

        private void InitActor(FPCharacterController fpCharacter)
        {
            fpCharacter.mass = 1000 / 1000f;
            fpCharacter.SetInteractiveFriction(interactiveFriction / 1000f);
            fpCharacter.SetInteractiveAccelerate(characterAccelerationFactor / 1000f);
            jumpHigh = 1200 / 1000f;
            moveSpeed = 6000 / 1000f;
            fpCharacter.onOffGround = () =>
            {
                if (characterState == CharacterState.Normal)
                {
                }
            };
            fpCharacter.onKnockBack = (force) =>
            {
                knockBackCooldown = 0;
                characterState = CharacterState.KnockBack;
            };
        }
        
        private void Update()
        {
            // 駆動PhysicsView
            FPPhysicsPresenter.Instance.OnViewUpdate();
            
            actor.SetInteractiveFriction(interactiveFriction / 1000f);
            actor.SetInteractiveAccelerate(characterAccelerationFactor / 1000f);
            actor.mass = mass;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        private void Jump()
        {
            if (characterState == CharacterState.Normal)
            {
                if ((actor.isGround || actor.fallDuration < 0.1) && actor.JumpAble())
                {
                    characterState = CharacterState.Jump;
                    actor.Jump(jumpHigh);
                }
                else
                {
                    if (movement.sqrMagnitude > 0.01)
                    {
                        Dive();
                    }
                }

            }
            else if (characterState == CharacterState.Jump)
            {
                if (movement.sqrMagnitude > FixedPoint64.EN6)
                {
                    Dive();
                }
            }
        }

        private void Dive()
        {
            characterState = CharacterState.Dive;
            diveSpeed = movement.magnitude;
        }
        
        private void FixedUpdate()
        {
            // 駆動Physics
            FPPhysicsPresenter.Instance.OnUpdate();
            
            if (actor.fpTransform.position.y < -60)
            {
                actor.fpTransform.position = startPosition;
                actor.Reset();
            }
            movement = FixedPointVector3.zero;
            float joyX, joyZ;
            {
                joyX = joyZ = 0;
            }
            joyX += Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
            joyZ += Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
            joyX += Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            joyZ += Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
            var v = new Vector2(joyX, joyZ).normalized;
            joyX = v.x;
            joyZ = v.y;
            var cameraTrans = mainCamera.transform;
            var forward = cameraTrans.forward;
            var right = cameraTrans.right;
            var deltaX = new Vector3(forward.x, 0, forward.z).normalized * joyX;
            var deltaZ = new Vector3(right.x, 0, right.z).normalized * joyZ;
            movement = new FixedPointVector3(deltaX + deltaZ) * moveSpeed * Time.fixedDeltaTime;
            //actor.mass = 1 + weight;
            switch (characterState)
            {
                case CharacterState.Normal:
                    actor.Move(movement * actor.reverseMass);
                    if (movement != FixedPointVector3.zero)
                    {
                        actor.transform.rotation = Quaternion.Lerp(actor.transform.rotation, Quaternion.LookRotation(movement.normalized.ToVector3(), Vector3.up), 0.5f);
                    }
                    break;
                case CharacterState.KnockBack:
                    actor.frictionKnockBack = knockBackCooldown * knockBackCooldown * knockBackCooldown;
                    knockBackCooldown += FPPhysicsPresenter.Instance.DeltaTime * 3 * actor.dampKnockBackDamp;
                    actor.transform.forward = -actor.knockBackVelocity.normalized.ToVector3();
                    if (actor.deltaPosition.magnitude < 0.001)
                    {
                        characterState = CharacterState.KnockBackGetUp;
                        knockBackUpDuration = 0;
                        actor.frictionKnockBack = 0.4;
                    }
                    break;
                case CharacterState.KnockBackGetUp:
                    if (knockBackUpDuration > knockBackGetUpTime)
                    {
                        characterState = CharacterState.Normal;
                    }
                    knockBackUpDuration += Time.fixedDeltaTime;
                    break;
                case CharacterState.Jump:
                    actor.Move(movement * actor.reverseMass);
                    if (movement != FixedPointVector3.zero)
                    {
                        actor.transform.rotation = Quaternion.Lerp(actor.transform.rotation, Quaternion.LookRotation(movement.normalized.ToVector3(), Vector3.up), 0.5f);
                    }
                    if (actor.transform.position.y < preTransformPos.y)
                    {
                    }
                    if (actor.isGround && actor.deltaPosition.y <= 0 && actor.forces != FixedPointVector3.zero)
                    {
                        characterState = CharacterState.Normal;
                    }
                    break;
                case CharacterState.Dive:
                    if (movement != FixedPointVector3.zero)
                    {
                        actor.transform.rotation = Quaternion.Lerp(actor.transform.rotation, Quaternion.LookRotation(movement.normalized.ToVector3(), Vector3.up), 0.5f);
                        actor.fpTransform.rotation = FixedPointQuaternion.LookRotation(movement.normalized,FixedPointVector3.up);
                    }
                    actor.Move(actor.fpTransform.forward * diveSpeed * 2 * actor.reverseMass + new FixedPointVector3(0, -diveSpeed, 0));
                    if (actor.isGround)
                    {
                        diveSpeed = FixedPointMath.Max(0, diveSpeed * 0.925);
                    }
                    if (actor.deltaPosition.magnitude < 0.1 && actor.isGround)
                    {
                    }
                    if (actor.deltaPosition.magnitude < 0.01 && actor.isGround)
                    {
                        characterState = CharacterState.DiveGetUp;
                        diveGetUpDuration = 0;
                        diveDuration = 0;
                    }
                    if (diveDuration > diveTime && actor.isGround)
                    {
                        characterState = CharacterState.Normal;
                        diveGetUpDuration = 0;
                        diveDuration = 0;
                    }
                    diveDuration += FPPhysicsPresenter.Instance.DeltaTime;
                    break;
                case CharacterState.DiveGetUp:
                    if (movement != FixedPointVector3.zero)
                    {
                        actor.transform.rotation = Quaternion.Lerp(actor.transform.rotation, Quaternion.LookRotation(movement.normalized.ToVector3(), Vector3.up), 0.5f);
                        actor.ChangeVelocityDirection(new FixedPointVector2(movement.x, movement.z).normalized);
                    }
                    if (diveGetUpDuration > diveGetUpTime)
                    {
                        characterState = CharacterState.Normal;
                    }
                    diveGetUpDuration += Time.fixedDeltaTime;
                    break;
                case CharacterState.Victory:
                    actor.transform.rotation = zeroQ;
                    break;
                case CharacterState.DoubleJump:
                    break;
            }
            preTransformPos = actor.transform.position;
        }
    }
}
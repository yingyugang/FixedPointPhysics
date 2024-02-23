namespace BlueNoah.PhysicsEngine
{
    using Math.FixedPoint;
    using UnityEngine;

    public class PushableHangePresenter : FPGameObject
    {
        [SerializeField] private bool isNotAwakeCollUpdate;

        [SerializeField] private FPBoxCollider obbCollide;

        [SerializeField] private FPBoxCollider obbCollide1;

        private FixedPointVector3 axis;
        private void Awake()
        {
            if (!isNotAwakeCollUpdate)
            {
                Init();
            }
        }

        protected override void Init()
        {
            axis = fpTransform.up;
            obbCollide.onCharacterCollide = OnPush;
            obbCollide1.onCharacterCollide = OnPush;
        }

        private void OnPush(FPCollision collision)
        {
            if (!collision.hit) return;
            var point = collision.contactPoint;
            var t = FixedPointVector3.Dot((point - fpTransform.position), axis);
            var closest = fpTransform.position + axis * t;
            var perpendicular = (point - closest).normalized;
            var tangent = FixedPointVector3.Cross(axis, perpendicular);
            var move = FixedPointVector3.Dot(collision.normal * collision.depth, tangent);
            var angle = move * FixedPoint64.Rad2Deg * 0.1;
            fpTransform.rotation = fpTransform.rotation * FixedPointQuaternion.AngleAxis(-angle, axis);
        }

        public override void OnLogicUpdate()
        {
        }

        public override void OnViewUpdate()
        {
            transform.rotation = fpTransform.rotation.ToQuaternion();
        }
    }
}
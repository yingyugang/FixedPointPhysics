namespace BlueNoah.PhysicsEngine
{
    using Math.FixedPoint;
    using UnityEngine;

    public class RotateWallPresenter : FPGameObject
    {
        [SerializeField] private FPBoxCollider obbCollide;
        [SerializeField] private FPBoxCollider obbCollide1;
        [SerializeField] private Transform rotateModelRoot;
        [SerializeField]
        [Range(-5000, 5000)]
        [Header("[Rotate speed.Min=-5000,Max=5000,1000 means 1degree/s]")]
        private int speed = 1000;
        private FixedPointQuaternion quaternion;
        private FixedPointVector3 axis;

        protected override void Init()
        {
            axis = fpTransform.up;
            quaternion = FixedPointQuaternion.AngleAxis(speed * 0.001, axis);
        }

        public override void OnLogicUpdate()
        {
            fpTransform.rotation = quaternion * fpTransform.rotation;
        }

        public override void OnViewUpdate()
        {
            rotateModelRoot.rotation = fpTransform.rotation.ToQuaternion();
        }
    }
}
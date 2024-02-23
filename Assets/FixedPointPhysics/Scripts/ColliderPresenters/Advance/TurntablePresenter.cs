using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlueNoah.PhysicsEngine
{
    public sealed class TurntablePresenter : FPGameObject
    {
        [FormerlySerializedAs("tableFbCylinder")] [FormerlySerializedAs("tableCylinderFb")] [FormerlySerializedAs("tableCylinder")] [SerializeField]
        private FPCylinderCollider tableFpCylinder;
        [FormerlySerializedAs("blockFbCylinder")] [FormerlySerializedAs("blockCylinderFb")] [FormerlySerializedAs("blockCylinder")] [SerializeField]
        private FPCylinderCollider blockFpCylinder;
        [SerializeField]
        private List<FPGameObject> childGimmicks;
        public int speed = 1000;
        private FixedPointVector3 axis;
        private FixedPointQuaternion quaternion;

        protected override void Init()
        {
            axis = (tableFpCylinder.endPos - tableFpCylinder.startPos).normalized;
            tableFpCylinder.onCharacterCollide = (collision) =>
            {
                if (FixedPointVector3.Dot(collision.normal, tableFpCylinder.fpTransform.up) > 0.9)
                {
                    var character = (FPCharacterController)collision.collider;
                    var delta = quaternion * (character.fpTransform.position - tableFpCylinder.startPos);
                    character.fpTransform.position = delta + tableFpCylinder.startPos;
                    var trans = character.transform;
                    trans.rotation = quaternion.ToQuaternion() * trans.rotation;
                }
            };
        }

        public override void OnLogicUpdate()
        {
            quaternion = FixedPointQuaternion.AngleAxis(speed * 0.001, axis);
            blockFpCylinder.fpTransform.rotation = quaternion * blockFpCylinder.fpTransform.rotation;
            foreach (var item in childGimmicks)
            {
                var delta = quaternion * (item.fpTransform.position - tableFpCylinder.startPos);
                item.fpTransform.position = delta + tableFpCylinder.startPos;
                var trans = item.transform;
                trans.rotation = quaternion.ToQuaternion() * trans.rotation;
            }
        }
        public override void OnViewUpdate()
        {
            
        }
    }
}
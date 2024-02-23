/*
* Create 2023/2/2
* 応彧剛　yingyugang@gmail.com
* Vertical Bumper
* It's used by fixed point physics system.
*/
using BlueNoah.Math.FixedPoint;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlueNoah.PhysicsEngine
{
    public sealed class VerticalBumperPresenter : FpBumperPresenter
    {
        [FormerlySerializedAs("fbCylinderCollider")] [FormerlySerializedAs("cylinderFbCollider")] [FormerlySerializedAs("cylinderCollider")] [FormerlySerializedAs("fixedPointCylinderColliderPresenter")] [SerializeField] private FPCylinderCollider fpCylinderCollider;
        [SerializeField] private int dampKnockBack = 2000;

        protected override void Init()
        {
            //fixedPointCylinderColliderPresenter.InitOrModifyCollider();
            FPPhysicsPresenter.Instance.fixedPointGameObjectFastList.Add(this);
            fpCylinderCollider.onCharacterCollide += (collision) =>
            {
                var character = (FPCharacterController)collision.collider;
                if (!VerifyCollisionInterval(character)) return;
                //Make sure only the around affect.
                if (FixedPointMath.Abs(FixedPointVector3.Dot(collision.normal, fpTransform.up)) >= 0.2) return;
                character.KnockBack((collision.normal) * force);
                //item.character.velocity = FixedPointVector3.zero;
                //item.character.AddForce(item.normal * force);
                character.dampKnockBackDamp = dampKnockBack * 0.001;
            };
        }

        public override void OnLogicUpdate()
        {
        }
        public override void OnViewUpdate()
        {
            
        }
    }
}
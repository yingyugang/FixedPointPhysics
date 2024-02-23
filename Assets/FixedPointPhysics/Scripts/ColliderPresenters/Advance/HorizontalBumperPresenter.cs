/*
* Create 2023/2/2 
* 応彧剛　yingyugang@gmail.com
* Horizontal Bumper
* It's used by fixed point physics system.
*/
using BlueNoah.Math.FixedPoint;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlueNoah.PhysicsEngine
{
    public sealed class HorizontalBumperPresenter : FpBumperPresenter
    {
        [FormerlySerializedAs("fbCylinderCollider")] [FormerlySerializedAs("cylinderFbCollider")] [FormerlySerializedAs("cylinderCollider")] [FormerlySerializedAs("fixedPointCylinderColliderPresenter")] [SerializeField] private FPCylinderCollider fpCylinderCollider;
        [SerializeField] private new Animation animation;
        
        private void Awake()
        {
            Init();
        }

        protected override void Init()
        {
            fpCylinderCollider.onCharacterCollide += (collision) =>
            {
                var character = (FPCharacterController)collision.collider;
                if (!VerifyCollisionInterval(character) || !active) return;
                //Make sure only the front affect.
                if (FixedPointVector3.Dot(collision.normal, fpTransform.up) <= 0.9) return;
                if(animation!=null&&!animation.isPlaying)
                    animation.Play();
                //var forward = new FixedPointVector3(item.character.deltaPosition.x, 0, item.character.deltaPosition.z).normalized;
                character.velocity = FixedPointVector3.zero;
                character.AddForce(collision.normal * force);
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
/*
* Create 2023/3/23
* 応彧剛　yingyugang@gmail.com
* It's used by fixedPoint physics system.
*/
using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public sealed class ConveyorBeltPresenter : FPGameObject
    {
        public FPBoxCollider beltOBB;
        public FPBoxCollider blockOBB;
        private bool directInit;
        public Material uvMat;
        public int matIndex;
        private Material mat;
        private Renderer blockRenderer;
        public bool isActive = true;
        public int speed = 1000;
        [Header("見た目の動く速度と人の搬送速度（speed）の間の比率、上がると見た目速度がより早くなる")]
        [SerializeField] private int speedUvSpeedRate = 15;
        private string uvProperty;

        protected override void Init()
        {
            directInit = true;
            mat = new Material(uvMat);
            blockRenderer = blockOBB.GetComponent<Renderer>();
            if (matIndex < blockRenderer.materials.Length)
            {
                blockRenderer.materials[matIndex] = mat;
            }
            else
            {
                blockRenderer.material = mat;
            }

            if (blockRenderer.materials[matIndex].HasProperty("_BaseMap"))
            {
                uvProperty = "_BaseMap";
            }
            else if (blockRenderer.materials[matIndex].HasProperty("_MainTex"))
            {
                uvProperty = "_MainTex";
            }
            beltOBB.onCharacterCollide += OnCollision;
        }

        private void OnCollision(FPCollision collision)
        {
            if (!isActive) return;
            //　側面のキャラ移動しない
            var dot = FixedPointVector3.Dot(collision.normal, collision.collider.fpTransform.up);
            if (dot > 0.99)
            {
                var character = (FPCharacterController)collision.collider;
                character.AddImpulse(Direct * FPPhysicsPresenter.Instance.DeltaTime);
            }
        }

        public override void OnViewUpdate()
        {
            //_BaseMap_ST
            //blockOBB.GetComponent<Renderer>().materials[matIndex].SetTextureOffset("_BaseMap", uvSpeedRate * (speed * 0.001f) * Time.time);
            if (uvProperty != null && isActive)
            {
                blockRenderer.materials[matIndex].SetTextureOffset(uvProperty, new Vector2(0, speed * speedUvSpeedRate * 0.000001f) * Time.time);
            }
        }

        public override void OnLogicUpdate()
        {
            
        }

        private FixedPointVector3 Direct
        {
            get
            {
                if (!directInit)
                {
                    Init();
                   
                }
                return beltOBB.fpTransform.forward * (speed * 0.001);
            }
        }
    }
}
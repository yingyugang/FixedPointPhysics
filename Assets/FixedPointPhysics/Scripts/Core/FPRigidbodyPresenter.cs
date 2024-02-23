/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixed point physics system.
*/
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    [ExecuteAlways]
    public class FPRigidbodyPresenter : MonoBehaviour
    {
        public FPRigidbody fpRigidbody { get; private set; }
        private FPSphereCollider fpSphereCollider;
        private void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            fpSphereCollider = GetComponent<FPSphereCollider>();
            //fixedPointSphereColliderPresenter.InitOrModifyCollider();
            fpSphereCollider.isDynamic = true;
            // TODO Put body into collider.
            fpRigidbody = new FPRigidbody(fpSphereCollider, fpSphereCollider.fpTransform);
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                fpRigidbody.enable = true;
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                fpRigidbody.enable = false;
            }
        }

        private void OnDestroy()
        {
            if (Application.isPlaying && FPPhysicsPresenter.Instance != null)
            {
                FPPhysicsPresenter.Instance.fixedPointRigidbodies.Remove(fpRigidbody);
            }
        }
    }
}
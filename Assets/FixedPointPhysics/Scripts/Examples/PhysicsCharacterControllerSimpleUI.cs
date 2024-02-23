using UnityEngine;
using UnityEngine.UI;

namespace BlueNoah.PhysicsEngine
{
    public class PhysicsCharacterControllerSimpleUI : MonoBehaviour
    {
        [SerializeField]
        private Button btnCapsuleCharacter;
        [SerializeField]
        private Button btnSphereCharacter;

        private void Awake()
        {
            btnCapsuleCharacter.onClick.AddListener(() =>
            {
                PhysicsCharacterControllerSimpleExample.Instance.SwitchToCapsule();
            });
            
            btnSphereCharacter.onClick.AddListener(() =>
            {
                PhysicsCharacterControllerSimpleExample.Instance.SwitchToSphere();
            });
        }
    }
}
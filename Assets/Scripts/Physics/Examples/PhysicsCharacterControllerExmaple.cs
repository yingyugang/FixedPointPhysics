using BlueNoah.Math.FixedPoint;
using UnityEngine;
using UnityEngine.UI;

namespace BlueNoah.PhysicsEngine
{
    public class PhysicsCharacterControllerExmaple : MonoBehaviour
    {
        [HideInInspector]
        public FixedPointVector3 orientation;
        [SerializeField]
        UltimateJoystick joystick;
        [SerializeField]
        Button jumpBtn;
        [SerializeField]
        Camera mainCamera;

        [HideInInspector]
        public FixedPointCharacterController actor;
    }
}
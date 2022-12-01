using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
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
        public FixedPointCharacterController actor;
        FixedPoint64 moveSpeed = 5;

        private void FixedUpdate()
        {
            var movement = FixedPointVector3.zero;
            if (joystick.GetJoystickState())
            {
                var deltaX = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z) * joystick.GetVerticalAxis();
                var deltaZ = new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z) * joystick.GetHorizontalAxis();
                movement = new FixedPointVector3(deltaX + deltaZ) * moveSpeed * Time.fixedDeltaTime;
            }
            actor.Move(movement);
        }
    }
}
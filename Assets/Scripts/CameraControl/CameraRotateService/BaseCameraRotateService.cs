using BlueNoah.Event;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.CameraControl
{
    [System.Serializable]
    public class BaseCameraRotateService
    {

        Camera mCamera;

        bool mIsCameraAutoRotate;

        float mRotateSpeed = 10f;

        bool mVerticalRotateable;

        bool mHorizontalRotateable;

        Vector3 mPreMousePos;

        BoxCollider mMoveArea;

        public float minAngle = 20;

        public float maxAngle = 85;

        public float currentAngle = 65;

        public BaseCameraRotateService(Camera camera)
        {
            mCamera = camera;
        }

        public float RotateSpeed
        {
            get
            {
                return mRotateSpeed;
            }
            set
            {
                mRotateSpeed = value;
            }
        }

        public bool IsCameraAutoRotate
        {
            get
            {
                return mIsCameraAutoRotate;
            }
        }

        public bool IsVerticalRotateable
        {
            get
            {
                return mVerticalRotateable;
            }
            set
            {
                mVerticalRotateable = value;
            }
        }

        public bool IsHorizontalRotateable
        {
            get
            {
                return mHorizontalRotateable;
            }
            set
            {
                mHorizontalRotateable = value;
            }
        }

        public void SetMoveArea(BoxCollider boxCollider)
        {
            mMoveArea = boxCollider;
        }

        public void OnUpdate()
        {

        }

        public void OnLateUpdate()
        {
            if (mIsCameraAutoRotate)
                return;
            if (mVerticalRotateable)
            {


                if (Input.GetMouseButton(1))
                {
                    HorizontalRotate(Input.GetAxis("Mouse X") * 2);
                    VerticalRotate(-Input.GetAxis("Mouse Y") * 2);
                }

                if (Input.GetKey(KeyCode.Z))
                {
                    if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))
                    {
                        return;
                    }
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        VerticalRotate(mRotateSpeed * Time.deltaTime * 2);
                    }
                    else
                    {
                        VerticalRotate(mRotateSpeed * Time.deltaTime);
                    }
                }
                if (Input.GetKey(KeyCode.X))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        VerticalRotate(-mRotateSpeed * Time.deltaTime * 2);
                    }
                    else
                    {
                        VerticalRotate(-mRotateSpeed * Time.deltaTime);
                    }
                }
            }
            if (mHorizontalRotateable)
            {
                if (Input.GetKey(KeyCode.Q))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        HorizontalRotate(mRotateSpeed * Time.deltaTime * 2);
                    }
                    else
                    {
                        HorizontalRotate(mRotateSpeed * Time.deltaTime);
                    }
                }
                if (Input.GetKey(KeyCode.E))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        HorizontalRotate(-mRotateSpeed * Time.deltaTime * 2);
                    }
                    else
                    {
                        HorizontalRotate(-mRotateSpeed * Time.deltaTime);
                    }
                }
            }
        }

        public void HorizontalRotate(float angle)
        {
            Vector3 pos = CameraController.GetCameraForwardPosition(mCamera);
            Quaternion endQuaternion;
            Vector3 endPos;
            RotateAround(mCamera.transform, pos, new Vector3(0, 1, 0), angle, out endQuaternion, out endPos);
            mCamera.transform.position = endPos;
            mCamera.transform.rotation = endQuaternion;
        }

        float CurrentVerticalAngle
        {
            get
            {
                Vector3 forward = mCamera.transform.forward;
                Vector3 forward1 = new Vector3(forward.x, 0, forward.z).normalized;
                return Vector3.Angle(forward, forward1);
            }
        }

        public void VerticalRotate(float angle)
        {
            Vector3 pos = CameraController.GetCameraForwardPosition(mCamera);
            float angle1 = Mathf.Clamp(CurrentVerticalAngle + angle, minAngle, maxAngle);
            angle = angle - (CurrentVerticalAngle + angle - angle1);
            Quaternion endQuaternion;
            Vector3 endPos;
            RotateAround(mCamera.transform, pos, mCamera.transform.right, angle, out endQuaternion, out endPos);
            Vector3 forward = endQuaternion * Vector3.forward;
            Vector3 forward1 = new Vector3(forward.x, 0, forward.z).normalized;
            float targetAngle = Vector3.Angle(forward, forward1);
            mCamera.transform.rotation = endQuaternion;
            mCamera.transform.position = endPos;
        }

        public void DOVerticalRotate(Vector3 pos, float angle, float rotateDuration, UnityAction onComplete = null)
        {
            mIsCameraAutoRotate = true;
            Vector3 endPos;
            Quaternion endQuaternion;
            RotateAround(mCamera.transform, pos, mCamera.transform.right, angle, out endQuaternion, out endPos);
            mCamera.transform.DOMove(endPos, rotateDuration).OnComplete(() =>
            {
                mIsCameraAutoRotate = false;
                if (onComplete != null)
                {
                    onComplete();
                }
            });
            mCamera.transform.DORotateQuaternion(endQuaternion, rotateDuration);
        }

        public void DOVerticalRotate(Vector3 endPos, Vector3 endAngle, float rotateDuration, UnityAction onComplete = null)
        {
            mIsCameraAutoRotate = true;
            CameraController.Instance.IsMoveable = false;
            CameraController.Instance.IsControllable = false;
            mCamera.transform.DOMove(endPos, rotateDuration).OnComplete(() =>
            {
                CameraController.Instance.IsMoveable = true;
                CameraController.Instance.IsControllable = true;
                mIsCameraAutoRotate = false;
                if (onComplete != null)
                {
                    onComplete();
                }
            });
            mCamera.transform.DORotate(endAngle, rotateDuration);
        }

        public static void RotateAround(Transform trans, Vector3 center, Vector3 axis, float angle, out Quaternion targetQuaternion, out Vector3 targetPosition)
        {
            Vector3 pos = trans.position;
            Quaternion rot = Quaternion.AngleAxis(angle, axis);
            Vector3 dir = pos - center;
            dir = rot * dir;
            targetPosition = center + dir;
            Quaternion myRot = trans.rotation;
            targetQuaternion = trans.rotation * Quaternion.Inverse(myRot) * rot * myRot;
        }

        public static Vector3 Rotate(Vector3 pos, Vector3 axis, float angle)
        {
            Quaternion quaternion = new Quaternion();
            quaternion.x = axis.x * Mathf.Sin(angle / 360 * Mathf.PI);
            quaternion.y = axis.y * Mathf.Sin(angle / 360 * Mathf.PI);
            quaternion.z = axis.z * Mathf.Sin(angle / 360 * Mathf.PI);
            quaternion.w = Mathf.Cos(angle / 360 * Mathf.PI);
            return quaternion * pos;
        }

    }
}

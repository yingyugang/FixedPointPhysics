using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using BlueNoah.Event;
using Unity.VisualScripting;

namespace BlueNoah.CameraControl
{
    [DefaultExecutionOrder(-100)]
    public class CameraController : SimpleSingleMonoBehaviour<CameraController>
    {
        bool mIsControllable = true;
        bool mIsMoveable = true;
        Camera mCamera;
        BoxCollider mMoveArea;
        BaseCameraPinchService mBaseCameraPinchService;
        BaseCameraRotateService mBaseCameraRotateService;
        BaseCameraMoveService mBaseCameraMoveService;
        public float baseDistance = 48f;
        protected float zoomFactor = 4.0f;

        protected override void Awake()
        {
            base.Awake();
            mCamera = GetComponent<Camera>();
            if (mCamera == null)
                mCamera = Camera.main;

            mBaseCameraRotateService = new BaseCameraRotateService(mCamera);

            if (mCamera.orthographic)
            {
                mBaseCameraMoveService = new OrthographicCameraMoveService(mCamera);
                mBaseCameraPinchService = new OrthographicsCameraPinchService(mCamera);
            }
            else
            {
                mBaseCameraMoveService = new PerspectiveCameraMoveService(mCamera);
                mBaseCameraPinchService = new PerspectiveCameraPinchService(mCamera);
                mBaseCameraPinchService.currentDistance = GetCameraForwardDistance(mCamera);
            }

            mBaseCameraMoveService.Init();
            mBaseCameraPinchService.Init();

            gameObject.GetOrAddComponent<EasyInput>();
        }

        void Start()
        {

            EasyInput.Instance.AddLateUpdateListener(Event.TouchType.TouchBegin, 0, OnTouchBegin);

            EasyInput.Instance.AddLateUpdateListener(Event.TouchType.Touch, 0, OnTouch);

            EasyInput.Instance.AddLateUpdateListener(Event.TouchType.TouchEnd, 0, OnTouchEnd);

            EasyInput.Instance.AddGlobalLateUpdateListener(Event.TouchType.TwoFingerBegin, OnTowFingerBegin);

            EasyInput.Instance.AddGlobalLateUpdateListener(Event.TouchType.TwoFinger, OnPinch);

            EasyInput.Instance.AddGlobalLateUpdateListener(Event.TouchType.TwoFinger, OnRotate);

            EasyInput.Instance.AddGlobalLateUpdateListener(Event.TouchType.TwoFingerEnd, OnTwoFingerEnd);
        }

        public void Initialize(Vector3 pos,Vector3 rot, Vector3 range)
        {
            mCamera.transform.position = pos;
            mCamera.transform.eulerAngles = rot;
            mBaseCameraPinchService.currentDistance = GetCameraForwardDistance(mCamera);
            mBaseCameraMoveService.SetMoveRange(range);
        }

        public float MinPinchSize
        {
            set
            {
                mBaseCameraPinchService.minSize = value;
            }
            get
            {
                return mBaseCameraPinchService.minSize;
            }
        }

        public float MaxPinchSize
        {
            set
            {
                mBaseCameraPinchService.maxSize = value;
            }
            get
            {
                return mBaseCameraPinchService.maxSize;
            }
        }

        public bool IsControllable
        {
            get
            {
                return mIsControllable;
            }
            set
            {
                mIsControllable = value;
            }
        }

        public bool IsMoveable
        {
            get
            {
                return mIsMoveable;
            }
            set
            {
                mIsMoveable = value;
                if (!mIsMoveable)
                {
                    mBaseCameraMoveService.ClearRemainDistance();
                }
            }
        }

        public bool IsPinchable
        {
            get
            {
                return mBaseCameraPinchService.IsPinchable;
            }
            set
            {
                mBaseCameraPinchService.IsPinchable = value;
            }
        }

        public bool IsVerticalRotateable
        {
            get
            {
                return mBaseCameraRotateService.IsVerticalRotateable;
            }
            set
            {
                mBaseCameraRotateService.IsVerticalRotateable = value;
            }
        }

        public bool IsHorizontalRotateable
        {
            get
            {
                return mBaseCameraRotateService.IsHorizontalRotateable;
            }
            set
            {
                mBaseCameraRotateService.IsHorizontalRotateable = value;
            }
        }

        public bool IsKeyboardMoveable
        {
            get
            {
                return mBaseCameraMoveService.IsKeyboardMoveable;
            }
            set
            {
                mBaseCameraMoveService.IsKeyboardMoveable = value;
            }
        }

        public bool IsScreenEdgeMoveable
        {
            get
            {
                return mBaseCameraMoveService.IsScreenEdgeMoveable;
            }
            set
            {
                mBaseCameraMoveService.IsScreenEdgeMoveable = value;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return mBaseCameraMoveService.MoveSpeed;
            }
            set
            {
                mBaseCameraMoveService.MoveSpeed = value;
            }
        }

        public float RotateSpeed
        {
            get
            {
                return mBaseCameraRotateService.RotateSpeed;
            }
            set
            {
                mBaseCameraRotateService.RotateSpeed = value;
            }
        }

        public void SetCameraMoveArea(BoxCollider boxCollider)
        {
            mBaseCameraMoveService.SetMoveArea(boxCollider);
            mBaseCameraRotateService.SetMoveArea(boxCollider);
        }

        void OnTouchBegin(EventData eventData)
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if (!eventData.currentTouch.isPointerOnGameObject)
                {
                    mBaseCameraMoveService.CancelMove();
                    mBaseCameraPinchService.CancelPinch();
                    if (!mBaseCameraRotateService.IsCameraAutoRotate && mIsControllable && IsMoveable && !mBaseCameraPinchService.IsPinching)
                    {
                        mBaseCameraMoveService.MoveBegin(eventData);
                    }
                }
            }
        }

        void OnTouch(EventData eventData)
        {
            if (!mBaseCameraRotateService.IsCameraAutoRotate && mIsControllable && IsMoveable && !mBaseCameraPinchService.IsPinching)
            {
                mBaseCameraMoveService.Move(eventData);
            }
        }

        void OnTouchEnd(EventData eventData)
        {
            if (!mBaseCameraRotateService.IsCameraAutoRotate && mIsControllable && IsMoveable && !mBaseCameraPinchService.IsPinching)
            {
                mBaseCameraMoveService.MoveEnd(eventData);
            }
        }

        void OnRotateBegin(EventData eventData)
        {
            preDirect = null;
        }
        // void OnRotate(EventData eventData)
        // {
        //     float angle = Vector3.Angle(eventData.touchPos1 - eventData.touchPos0, new Vector3(1, 0, 0));
        //     if (mBaseCameraRotateService.IsHorizontalRotateable)
        //     {
        //         mBaseCameraRotateService.HorizontalRotate(eventData.deltaAngle * 0.5f);
        //     }
        //     if (mBaseCameraRotateService.IsVerticalRotateable)
        //     {
        //         if (Mathf.Abs(eventData.deltaTouchPos0.x / eventData.deltaTouchPos0.y) < 0.3f && Mathf.Abs(eventData.deltaTouchPos1.x / eventData.deltaTouchPos1.y) < 0.3f)
        //         {
        //             float y = 0;
        //             if (eventData.deltaTouchPos0.y > 0 && eventData.deltaTouchPos1.y > 0)
        //             {
        //                 y = Mathf.Min(eventData.deltaTouchPos0.y, eventData.deltaTouchPos1.y);
        //             }
        //             if (eventData.deltaTouchPos0.y < 0 && eventData.deltaTouchPos1.y < 0)
        //             {
        //                 y = Mathf.Max(eventData.deltaTouchPos0.y, eventData.deltaTouchPos1.y);
        //             }
        //             mBaseCameraRotateService.VerticalRotate(y * 0.5f);
        //         }
        //     }
        // }
        void OnTowFingerBegin(EventData eventData)
        {
            mBaseCameraMoveService.CancelMove();
            mBaseCameraPinchService.CancelPinch();
            if (IsPinchable && IsControllable)
            {
                mBaseCameraPinchService.OnPinchBegin();
            }
            mBaseCameraPinchService.currentDistance = GetCameraForwardDistance(mCamera);
            OnRotateBegin(eventData);
        }

        void OnPinch(EventData eventData)
        {
            if (IsPinchable && IsControllable)
            {
                if (!isRotating)
                {
                    mBaseCameraPinchService.OnPinch(eventData);
                }
            }
        }
        bool isRotating;
        Vector2? preDirect;
        void OnRotate(EventData eventData)
        {
            if (IsControllable)
            {

                /* Rotate one
                var deltaPosition = (eventData.GetTouchData(0).touch.deltaPosition + eventData.GetTouchData(1).touch.deltaPosition) / 2f;
                if (IsVerticalRotateable)
                {
                    if (Screen.width < Screen.height)
                    {
                        mBaseCameraRotateService.VerticalRotate(deltaPosition.y / Screen.dpi * 2.54f);
                    }
                    else
                    {
                        mBaseCameraRotateService.VerticalRotate(deltaPosition.y / Screen.dpi * 2.54f * ((float)Screen.height / Screen.width));
                    }
                }
                if (IsHorizontalRotateable)
                {
                    if (Screen.width < Screen.height)
                    {
                        mBaseCameraRotateService.HorizontalRotate(deltaPosition.x / Screen.dpi * 2.54f * ((float)Screen.height / Screen.width));
                    }
                    else
                    {
                        mBaseCameraRotateService.HorizontalRotate(deltaPosition.x / Screen.dpi * 2.54f);
                    }
                }*/
                if (IsVerticalRotateable)
                {

                    // Rotate two
                    var direct = (eventData.GetTouchData(1).touch.position - eventData.GetTouchData(0).touch.position).normalized;
                    if (preDirect != null)
                    {
                        var delta = Vector2.SignedAngle(preDirect.GetValueOrDefault(), direct);
                        if (Mathf.Abs(delta) > 2f || (isRotating && Mathf.Abs(delta) > 0.1f))
                        {
                            mBaseCameraRotateService.HorizontalRotate(delta);
                            preDirect = direct;
                            isRotating = true;
                            return;
                        }
                    }
                    preDirect = direct;
                }
            }
        }

        void OnTwoFingerEnd(EventData eventData)
        {
            if (IsPinchable && IsControllable)
            {
                mBaseCameraPinchService.OnPinchEnd();
            }
            isRotating = false;
        }

        public Camera CurrentCamera
        {
            get
            {
                return mCamera;
            }
        }

        public void SetOrthoSize(float targetOrthograpphicSize)
        {
            mCamera.orthographicSize = targetOrthograpphicSize;
        }

        public void SetEulerAngle(Vector3 angle)
        {
            mCamera.transform.eulerAngles = angle;
        }

        public void DOOrthoSize(float targetOrthographicSize, float zoomDuration)
        {
            mCamera.DOOrthoSize(targetOrthographicSize, zoomDuration);
        }

        public void DOVerticalRotate(Vector3 pos, float angle, float rotateDuration, UnityAction onComplete = null)
        {
            mBaseCameraRotateService.DOVerticalRotate(pos, angle, rotateDuration, onComplete);
        }

        public void DOVerticalRotate(Vector3 pos, Vector3 angle, float rotateDuration, UnityAction onComplete = null)
        {
            mBaseCameraRotateService.DOVerticalRotate(pos, angle, rotateDuration, onComplete);
        }

        public void DOMove(Vector3 offset, float x, float y, float moveDuration, UnityAction onComplete)
        {
            Vector3 forward = mCamera.transform.forward;
            forward = new Vector3(forward.x, 0, forward.z).normalized;
            offset = offset + mCamera.orthographicSize * forward * x + Screen.width / (float)Screen.height * mCamera.orthographicSize * mCamera.transform.right * y;
            Vector3 targetPos = offset + mCamera.transform.position;
            mCamera.transform.DOMove(targetPos, moveDuration).OnComplete(() =>
            {
                if (onComplete != null)
                    onComplete();
            });
        }

        Vector3 GetOffsetPosition(GameObject go, Vector3 forword, float distance)
        {
            forword = new Vector3(forword.x, 0, forword.z).normalized;
            return go.transform.position - forword * distance;
        }

        public float GetCameraDistance()
        {
            return GetObjectOffset(mCamera.gameObject).magnitude;
        }

        Vector3 GetObjectOffset(GameObject go)
        {
            Vector3 position = GetCameraForwardPosition(mCamera);
            return go.transform.position - position;
        }

        public bool RaycastForward(Vector3 start, out RaycastHit raycastHit, int layer)
        {
            return Physics.Raycast(start, mCamera.transform.forward, out raycastHit, Mathf.Infinity, 1 << layer);
        }

        public Vector3 GetOrthographicToGridPositionWithOffset(int layer, float x, float y)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(GetOrthographicPositionWithOffset(x, y), mCamera.transform.forward, out raycastHit, Mathf.Infinity, 1 << layer))
            {
                return raycastHit.point;
            }
            return Vector3.zero;
        }

        Vector3 GetOrthographicPositionWithOffset(float x, float y)
        {
            float sizePerPixel = mCamera.orthographicSize * 2 / Screen.height;
            return mCamera.transform.position - mCamera.transform.up * y - mCamera.transform.right * x;
        }

        public Vector3 GetCameraForwardPosition()
        {
            return GetCameraForwardPosition(mCamera);
        }

        public static Vector3 GetCameraForwardPosition(Camera currentCamera)
        {
            RaycastHit raycastHit;
            Vector3 pos = Vector3.zero;
            if (Physics.Raycast(currentCamera.transform.position, currentCamera.transform.forward, out raycastHit, Mathf.Infinity, 1 << LayerConstant.GroundLayer))
            {
                pos = raycastHit.point;
            }
            else
            {
                pos = GetIntersectWithLineAndPlane(currentCamera.transform.position, currentCamera.transform.forward,Vector3.up,Vector3.zero);
            }
            return pos;
        }

        public float GetCameraForwardDistance(Camera currentCamera)
        {
            Vector3 pos = GetCameraForwardPosition(currentCamera);
            return Vector3.Distance(currentCamera.transform.position,pos);
        }
        public void SetCameraDistance(float distance)
        {
            mCamera.transform.position = GetCameraForwardPosition() - mCamera.transform.forward * distance;
        }

        public Vector3 ScreenPositionToOrthograhicCameraPosition()
        {
            float sizePerPixel = mCamera.orthographicSize * 2 / Screen.height;
            float x = (EasyInput.Instance.GetTouchPosition(0).x - Screen.width / 2) * sizePerPixel;
            float y = (EasyInput.Instance.GetTouchPosition(0).y - Screen.height / 2) * sizePerPixel;
            return mCamera.transform.position + mCamera.transform.up * y + mCamera.transform.right * x;
        }

        public bool GetWorldPositionByMousePosition(out RaycastHit raycastHit, int layer)
        {
            if (mCamera.orthographic)
            {
                return GetWorldTransFromMousePositionByOrthographicCamera(out raycastHit, layer);
            }
            else
            {
                return GetWorldTransFromMousePositionByPerspectiveCamera(out raycastHit, layer);
            }
        }

        public bool GetWorldPoistionByScreenPos(out RaycastHit raycastHit, Vector3 mousePosition, int layer)
        {
            mousePosition.z = 10;
            Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 forward = (position - Camera.main.transform.position).normalized;
            return Physics.Raycast(Camera.main.transform.position, forward, out raycastHit, Mathf.Infinity, layer);
        }

        bool GetWorldTransFromMousePositionByOrthographicCamera(out RaycastHit raycastHit, int layer)
        {
            Vector3 pos = ScreenPositionToOrthograhicCameraPosition();
            if (Physics.Raycast(pos, mCamera.transform.forward, out raycastHit, Mathf.Infinity, 1 << layer))
            {
                return true;
            }
            return false;
        }

        bool GetWorldTransFromMousePositionByPerspectiveCamera(out RaycastHit raycastHit, int layer)
        {
            Vector3 mousePosition = Input.mousePosition;

            mousePosition.z = 10;

            Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector3 forward = (position - Camera.main.transform.position).normalized;

            return Physics.Raycast(Camera.main.transform.position, forward, out raycastHit, Mathf.Infinity, 1 << layer);
        }

        public bool RaycastByOrthographicCamera(out RaycastHit raycastHit)
        {
            Vector3 pos = ScreenPositionToOrthograhicCameraPosition();
            if (Physics.Raycast(pos, mCamera.transform.forward, out raycastHit, Mathf.Infinity))
            {
                return true;
            }
            return false;
        }

        public RaycastHit[] RaycastAllByOrthographicCamera(int layer)
        {
            Vector3 pos = ScreenPositionToOrthograhicCameraPosition();
            return Physics.RaycastAll(pos, mCamera.transform.forward, Mathf.Infinity, 1 << layer);
        }

        public RaycastHit[] RaycastAllByOrthographicCameraWithLayers(int layers)
        {
            Vector3 pos = ScreenPositionToOrthograhicCameraPosition();
            return Physics.RaycastAll(pos, mCamera.transform.forward, Mathf.Infinity, layers);
        }

        public Vector3 GetCameraForward()
        {
            return new Vector3(mCamera.transform.forward.x, 0, mCamera.transform.forward.z).normalized;
        }

        public Vector3 GetCameraRight()
        {
            return new Vector3(mCamera.transform.right.x, 0, mCamera.transform.right.z).normalized;
        }

        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct, planeNormal);
            return d * direct.normalized + point;
        }

        void Update()
        {
            mBaseCameraMoveService.OnUpdate();
#if UNITY_EDITOR || UNITY_STANDALONE
            mBaseCameraPinchService.OnUpdate();
            mBaseCameraRotateService.OnUpdate();
#endif
        }

        void LateUpdate()
        {
            //mBaseCameraMoveService.OnLateUpdate();
#if UNITY_EDITOR || UNITY_STANDALONE
            mBaseCameraPinchService.OnLateUpdate();
            mBaseCameraRotateService.OnLateUpdate();
#endif
        }

        private void FixedUpdate()
        {
            mBaseCameraMoveService.OnFixedUpdate();
        }

        void OnDrawGizmos()
        {
            if (mBaseCameraMoveService != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(mBaseCameraMoveService.pos0, 0.1f);
                Gizmos.DrawSphere(mBaseCameraMoveService.pos1, 0.1f);
                Gizmos.DrawSphere(mBaseCameraMoveService.pos2, 0.1f);
                Gizmos.DrawSphere(mBaseCameraMoveService.pos3, 0.1f);
                Gizmos.color = Color.white;
            }
        }

        public void SetMoveRange(Vector3 vector)
        {
            mBaseCameraMoveService.SetMoveRange(vector);
        }
    }
}
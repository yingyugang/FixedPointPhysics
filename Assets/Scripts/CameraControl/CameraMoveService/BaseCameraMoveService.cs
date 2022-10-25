using UnityEngine;
using BlueNoah.Event;

namespace BlueNoah.CameraControl
{
    //Remain Distanceを使うと、操作の紐つりが少しだけなくなる。（Target Position を使わない）
    [System.Serializable]
    public abstract class BaseCameraMoveService
    {

        protected Camera mCamera;
        protected float mMoveSpeed = 2f;
        protected float mSmooth = 30f;
        protected float mMaxOverDistance = 0.05f;
        protected Vector3 mRemainRightDistance;
        protected Vector3 mRemainForwardDistance;
        protected BoxCollider mMoveArea;
        protected bool mIsTouching;
        bool mMoveBackable = true;
        protected bool mKeyboardMoveable = true;
        protected float mKeyboardSpeed = 1000;
        protected Vector3 planeNormal = new Vector3(0, 1, 0);
        protected Vector3 planeNormalPoint = new Vector3(0, 0, 0);

        protected bool mIsScreenEdgeMoveable;
        protected float moveSpeedRate = 1;

        protected Vector3 moveRange = new Vector3(75, 0, 75);

        // inertia
        protected Vector3 inertia = Vector3.zero;
        protected Vector3 deltaInertia = Vector3.zero;
        protected Vector3 inertiaBefore = Vector3.zero;
        protected Vector3 inertiaAfter = Vector3.zero;
        readonly protected int inertiaCount = 25;
        readonly protected float inertiaRatio = 0.25f;
        protected int inertiaCounter = 0;

        public void CancelMove()
        {
            mIsTouching = false;
        }

        public bool IsKeyboardMoveable
        {
            get
            {
                return mKeyboardMoveable;
            }
            set
            {
                mKeyboardMoveable = value;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return mMoveSpeed;
            }
            set
            {
                mMoveSpeed = value;
            }
        }

        public bool IsScreenEdgeMoveable
        {
            get
            {
                return mIsScreenEdgeMoveable;
            }
            set
            {
                mIsScreenEdgeMoveable = value;
            }
        }

        public void MoveBegin(EventData eventData)
        {
            mIsTouching = true;
        }

        public void ClearRemainDistance()
        {
            mRemainRightDistance = Vector3.zero;
            mRemainForwardDistance = Vector3.zero;
        }

        public void SetMoveArea(BoxCollider boxCollider)
        {
            this.mMoveArea = boxCollider;
        }

        public void MoveEnd(EventData eventData)
        {
            mIsTouching = false;
        }

        public virtual void Init()
        {
            EasyInput.Instance.AddListener(Event.TouchType.Touch, 0, ScreenEdgeMove);
        }

        public void OnUpdate()
        {

        }

        public void OnLateUpdate()
        {
            if (mKeyboardMoveable)
                KeyboardMove(Time.deltaTime);
            Vector3 detalForwardMove = mRemainForwardDistance * Time.deltaTime * mSmooth;
            Vector3 detalRightMove = mRemainRightDistance * Time.deltaTime * mSmooth;
            Vector3 targetPos = mCamera.transform.position + detalForwardMove + detalRightMove;
            if (mRemainForwardDistance.magnitude < detalForwardMove.magnitude)
            {
                mRemainForwardDistance = Vector3.zero;
            }
            else
            {
                mRemainForwardDistance -= detalForwardMove;
            }
            if (mRemainRightDistance.magnitude < detalRightMove.magnitude)
            {
                mRemainRightDistance = Vector3.zero;
            }
            else
            {
                mRemainRightDistance -= detalRightMove;
            }
            mCamera.transform.position = targetPos;
            //Do move back
            if (!mIsTouching && mMoveBackable)
                MoveBack();
        }

        public void OnFixedUpdate()
        {
            if (mKeyboardMoveable)
                KeyboardMove(Time.fixedDeltaTime);
            Vector3 detalForwardMove = mRemainForwardDistance * Time.fixedDeltaTime * mSmooth;
            Vector3 detalRightMove = mRemainRightDistance * Time.fixedDeltaTime * mSmooth;
            Vector3 targetPos = mCamera.transform.position + detalForwardMove + detalRightMove;
            if (mRemainForwardDistance.magnitude < detalForwardMove.magnitude)
            {
                mRemainForwardDistance = Vector3.zero;
            }
            else
            {
                mRemainForwardDistance -= detalForwardMove;
            }
            if (mRemainRightDistance.magnitude < detalRightMove.magnitude)
            {
                mRemainRightDistance = Vector3.zero;
            }
            else
            {
                mRemainRightDistance -= detalRightMove;
            }

            if (targetPos.x > moveRange.x * 0.5f)
            {
                targetPos.x = moveRange.x * 0.5f;
            }
            else if (targetPos.x < -moveRange.x * 0.5f)
            {
                targetPos.x = -moveRange.x * 0.5f;
            }

            float xAngle = mCamera.transform.eulerAngles.x;
            float dis = Mathf.Cos(xAngle / 180 * Mathf.PI) * (Mathf.Sqrt((-moveRange.z * 0.5f * -moveRange.z * 0.5f) + (mCamera.transform.position.y * mCamera.transform.position.y)));

            if (targetPos.z > (moveRange.z * 0.5f) + dis)
            {
                targetPos.z = moveRange.z * 0.5f + dis;
            }
            else if (targetPos.z < (-moveRange.z * 0.5f) + dis)
            {
                targetPos.z = -moveRange.z * 0.5f + dis;
            }

            if (mIsTouching && mCamera.transform.position != targetPos)
            {
                inertiaBefore = inertiaAfter;
                inertiaAfter = targetPos;
                inertia = (inertiaAfter - inertiaBefore) * inertiaRatio;
                deltaInertia = inertia / inertiaCount;
            }

            if(!mIsTouching && inertia != Vector3.zero)
            {
                mCamera.transform.position = targetPos + inertia;
                inertiaCounter++;
                inertia -= deltaInertia;
                if (inertiaCounter >= inertiaCount)
                {
                    inertia = Vector3.zero;
                    inertiaCounter = 0;
                }
            }
            else
            {
                mCamera.transform.position = targetPos;
            }



            //Do move back
            if (!mIsTouching && mMoveBackable)
                MoveBack();
        }

        //ズームの時も
        void MoveBack()
        {
            Vector3 offset = GetMoveAreOffset(mCamera.transform.position);
            mCamera.transform.position -= offset * Time.deltaTime * mSmooth / 2f;
        }

        public void Move(EventData eventData)
        {
            if (mIsTouching)
            {
                MoveCamera(eventData);
            }
        }

        public virtual void MoveCamera(EventData eventData)
        {
            var x = eventData.currentTouch.touch.deltaPosition.x;
            var y = eventData.currentTouch.touch.deltaPosition.y;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                x *= 2;
                y *= 2;
            }
            Vector3 forward = GetCameraForward();

            Vector3 right = GetCameraRight();

            float angle = Vector3.Angle(mCamera.transform.forward, new Vector3(0, -1, 0));

            Vector3 offsetOverArea = GetMoveAreOffset(mCamera.transform.position + mRemainForwardDistance + mRemainRightDistance);

            bool isSameForwardDirect = false;

            bool isSameRightDirect = false;

            if ((Vector3.Dot(Vector3.Project(offsetOverArea, forward).normalized, forward) > 0 && y < 0) || (Vector3.Dot(Vector3.Project(offsetOverArea, forward).normalized, forward) < 0 && y > 0))
            {
                isSameForwardDirect = true;
            }

            if ((Vector3.Dot(Vector3.Project(offsetOverArea, right).normalized, right) > 0 && x < 0) || (Vector3.Dot(Vector3.Project(offsetOverArea, right).normalized, right) < 0 && x > 0))
            {
                isSameRightDirect = true;
            }

            float forwardOverDistance = 0;

            if (isSameForwardDirect)
            {
                forwardOverDistance = Vector3.Project(offsetOverArea, forward).magnitude;
            }

            float rightOverDistance = 0;

            if (isSameRightDirect)
            {
                rightOverDistance = Vector3.Project(offsetOverArea, right).magnitude;
            }

            float forwardRadiu = Mathf.Cos(angle / 180f * Mathf.PI);

            float detalForwardDistance = y * Mathf.Max(0, Mathf.Cos((mMaxOverDistance / forwardRadiu - forwardOverDistance) / 2f * Mathf.PI / (mMaxOverDistance * 2 / forwardRadiu))) * mCamera.orthographicSize / Screen.height * mMoveSpeed * moveSpeedRate / forwardRadiu;

            float detalRightDistance = x * Mathf.Max(0, Mathf.Cos((mMaxOverDistance - rightOverDistance) / 2f * Mathf.PI / (mMaxOverDistance * 2))) * mCamera.orthographicSize / Screen.height * mMoveSpeed * moveSpeedRate;

            float forwardDistance = (mRemainForwardDistance - forward * detalForwardDistance).magnitude;

            float rightDistance = (mRemainRightDistance - right * detalRightDistance).magnitude;

            mRemainForwardDistance -= forward * detalForwardDistance;

            mRemainRightDistance -= right * detalRightDistance;

            //mRemainForwardDistance -= forward * y * Mathf.Max(0,Mathf.Cos((mSmoothDistance / forwardRadiu - forwardFloat) / 2f * Mathf.PI / (mSmoothDistance * 2 / forwardRadiu) )) * DistancePerPixel * CameraSpeed / forwardRadiu;
            //mRemainForwardDistance -= forward * y * Mathf.Max(0, Mathf.Cos((mSmoothDistance / forwardRadiu - forwardFloat) / 2f * Mathf.PI / (mSmoothDistance * 2 / forwardRadiu))) * mCamera.orthographicSize / Screen.height * mMoveSpeed / forwardRadiu;
        }

        void KeyboardMove(float deltaTime)
        {
            //orthographicSizeと角度に基ついて移す速さを計算する、そして、どんなorthographicSizeでも、移動スピードがほぼ同じ。
            float x = 0;
            float y = 0;
            if (Input.GetKey(KeyCode.A))
            {
                x = mKeyboardSpeed * deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                x = -mKeyboardSpeed * deltaTime;
            }
            if (Input.GetKey(KeyCode.W))
            {
                y = -mKeyboardSpeed * deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                y = mKeyboardSpeed * deltaTime;
            }
            Vector3 forward = GetCameraForward();
            Vector3 right = GetCameraRight();
            float angle = Vector3.Angle(mCamera.transform.forward, new Vector3(0, -1, 0));
            Vector3 offsetOverArea = GetMoveAreOffset(mCamera.transform.position + mRemainForwardDistance + mRemainRightDistance);
            bool isSameForwardDirect = false;
            bool isSameRightDirect = false;
            if ((Vector3.Dot(Vector3.Project(offsetOverArea, forward).normalized, forward) > 0 && y < 0) || (Vector3.Dot(Vector3.Project(offsetOverArea, forward).normalized, forward) < 0 && y > 0))
            {
                isSameForwardDirect = true;
            }
            if ((Vector3.Dot(Vector3.Project(offsetOverArea, right).normalized, right) > 0 && x < 0) || (Vector3.Dot(Vector3.Project(offsetOverArea, right).normalized, right) < 0 && x > 0))
            {
                isSameRightDirect = true;
            }
            float forwardOverDistance = 0;
            if (isSameForwardDirect)
            {
                forwardOverDistance = Vector3.Project(offsetOverArea, forward).magnitude;
            }
            float rightOverDistance = 0;
            if (isSameRightDirect)
            {
                rightOverDistance = Vector3.Project(offsetOverArea, right).magnitude;
            }
            float forwardRadiu = Mathf.Cos(angle / 180f * Mathf.PI);
            float detalForwardDistance = y * Mathf.Max(0, Mathf.Cos((mMaxOverDistance / forwardRadiu - forwardOverDistance) / 2f * Mathf.PI / (mMaxOverDistance * 2 / forwardRadiu))) * mCamera.orthographicSize / Screen.height * mMoveSpeed * moveSpeedRate / forwardRadiu;
            float detalRightDistance = x * Mathf.Max(0, Mathf.Cos((mMaxOverDistance - rightOverDistance) / 2f * Mathf.PI / (mMaxOverDistance * 2))) * mCamera.orthographicSize / Screen.height * mMoveSpeed * moveSpeedRate;
            float forwardDistance = (mRemainForwardDistance - forward * detalForwardDistance).magnitude;
            float rightDistance = (mRemainRightDistance - right * detalRightDistance).magnitude;
            mRemainForwardDistance -= forward * detalForwardDistance;
            mRemainRightDistance -= right * detalRightDistance;
        }

        void ScreenEdgeMove(EventData eventData)
        {
            if (this.mIsScreenEdgeMoveable)
            {
                Vector2 position = eventData.currentTouch.touch.position;
                float x = 0;
                float y = 0;
                if (position.x <= Screen.dpi / 2.54f)
                {
                    x = mKeyboardSpeed * Time.deltaTime;
                }
                if (position.x >= Screen.width - Screen.dpi / 2.54f)
                {
                    x = -mKeyboardSpeed * Time.deltaTime;
                }
                if (position.y <= Screen.dpi / 2.54f)
                {
                    y = mKeyboardSpeed * Time.deltaTime;
                }
                if (position.y >= Screen.height - Screen.dpi / 2.54f)
                {
                    y = -mKeyboardSpeed * Time.deltaTime;
                }
                Move(eventData);
            }
        }

        protected Vector3 GetCameraForward()
        {
            return new Vector3(mCamera.transform.forward.x, 0, mCamera.transform.forward.z).normalized;
        }

        protected Vector3 GetCameraRight()
        {
            return new Vector3(mCamera.transform.right.x, 0, mCamera.transform.right.z).normalized;
        }

        public void SetMoveRange(Vector3 vector)
        {
            moveRange = vector;
        }

        protected abstract Vector3 GetMoveAreOffset(Vector3 targetPos);

        public Vector3 pos0;
        public Vector3 pos1;
        public Vector3 pos2;
        public Vector3 pos3;
    }
}
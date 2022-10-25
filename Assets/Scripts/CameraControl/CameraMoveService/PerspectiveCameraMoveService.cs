using BlueNoah.Event;
using UnityEngine;

namespace BlueNoah.CameraControl
{
    [System.Serializable]
    public class PerspectiveCameraMoveService : BaseCameraMoveService
    {
        public PerspectiveCameraMoveService(Camera camera)
        {
            mCamera = camera;
            this.mMoveSpeed = 0.5f;
        }

        public override void MoveCamera(EventData eventData)
        {
            moveSpeedRate = CameraController.Instance.GetCameraDistance() / CameraController.Instance.baseDistance;
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
            //var currentX = eventData.currentTouch.touch.position.x;
            //var currentY = eventData.currentTouch.touch.position.y;
            //var degree = Vector3.Angle(- mCamera.transform.forward,Vector3.up);
            //detalRightDistance *= (currentY / Mathf.Tan(degree/180 * Mathf.PI) * 2 + Screen.width) / Screen.width ;
            mRemainForwardDistance -= forward * detalForwardDistance;
            mRemainRightDistance -= right * detalRightDistance;
        }

        protected override Vector3 GetMoveAreOffset(Vector3 targetPos)
		{
            Vector3 offset = Vector3.zero;
            if(mMoveArea!=null){
#if UNITY_EDITOR
                RaycastHit raycastHit;
                if (Physics.Raycast(NearTopRightCorner, (FarTopRightCorner - NearTopRightCorner).normalized, out raycastHit,Mathf.Infinity, 1 << LayerConstant.GroundLayer))
                {
                    pos0 = raycastHit.point;
                }
                if (Physics.Raycast(NearTopLeftCorner, (FarTopLeftCorner - NearTopLeftCorner).normalized, out raycastHit, Mathf.Infinity, 1 << LayerConstant.GroundLayer))
                {
                    pos1 = raycastHit.point;
                }
                if (Physics.Raycast(NearBottomLeftCorner, (FarBottomLeftCorner - NearBottomLeftCorner).normalized, out raycastHit, Mathf.Infinity, 1 << LayerConstant.GroundLayer))
                {
                    pos2 = raycastHit.point;
                }
                if (Physics.Raycast(NearBottomRightCorner, (FarBottomRightCorner - NearBottomRightCorner).normalized, out raycastHit, Mathf.Infinity, 1 << LayerConstant.GroundLayer))
                {
                    pos3 = raycastHit.point;
                }
#endif
                Vector3 offset0 = GetOffset(NearTopRightCorner, (FarTopRightCorner - NearTopRightCorner).normalized);
                Vector3 offset1 = GetOffset(NearTopLeftCorner, (FarTopLeftCorner - NearTopLeftCorner).normalized);
                Vector3 offset2 = GetOffset(NearBottomLeftCorner, (FarBottomLeftCorner - NearBottomLeftCorner).normalized);
                Vector3 offset3 = GetOffset(NearBottomRightCorner, (FarBottomRightCorner - NearBottomRightCorner).normalized);
                if (offset0.sqrMagnitude > offset.sqrMagnitude)
                {
                    offset = offset0;
                }
                if (offset1.sqrMagnitude > offset.sqrMagnitude)
                {
                    offset = offset1;
                }
                if (offset2.sqrMagnitude > offset.sqrMagnitude)
                {
                    offset = offset2;
                }
                if (offset3.sqrMagnitude > offset.sqrMagnitude)
                {
                    offset = offset3;
                }

            }
            return offset;
		}

        Vector3 GetOffset(Vector3 startPos, Vector3 forward)
        {

            Vector3 groundPosition = CameraController.GetIntersectWithLineAndPlane(startPos, forward, planeNormal, planeNormalPoint);

            Vector3 closePos = mMoveArea.ClosestPoint(groundPosition);

            Vector3 offset = groundPosition - closePos;

            return offset;
        }

        Vector3 NearTopRightCorner
        {
            get
            {
                return mCamera.ViewportToWorldPoint(new Vector3(1, 1, mCamera.nearClipPlane));
            }
        }

        Vector3 NearTopLeftCorner
        {
            get
            {
                return mCamera.ViewportToWorldPoint(new Vector3(0, 1, mCamera.nearClipPlane));
            }
        }

        Vector3 NearBottomLeftCorner
        {
            get
            {
                return mCamera.ViewportToWorldPoint(new Vector3(0, 0, mCamera.nearClipPlane));

            }
        }

        Vector3 NearBottomRightCorner
        {
            get
            {
                return mCamera.ViewportToWorldPoint(new Vector3(1, 0, mCamera.nearClipPlane));

            }
        }

        Vector3 FarTopRightCorner
        {
            get
            {
                return mCamera.ViewportToWorldPoint(new Vector3(1, 1, mCamera.farClipPlane));

            }
        }

        Vector3 FarTopLeftCorner
        {
            get
            {
                return mCamera.ViewportToWorldPoint(new Vector3(0, 1, mCamera.farClipPlane));

            }
        }

        Vector3 FarBottomRightCorner
        {
            get
            {
                return mCamera.ViewportToWorldPoint(new Vector3(1, 0, mCamera.farClipPlane));

            }
        }

        Vector3 FarBottomLeftCorner
        {
            get
            {
                return mCamera.ViewportToWorldPoint(new Vector3(0, 0, mCamera.farClipPlane));

            }
        }

    }
}

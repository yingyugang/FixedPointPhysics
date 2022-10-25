using UnityEngine;
using System.Collections;
using BlueNoah.Event;

namespace BlueNoah.CameraControl
{

    public class PerspectiveCameraPinchService : BaseCameraPinchService
    {

        private float mMinDistance = 10;
        private float mMaxDistance = 40;
        private float mCurrentDistance;

        public override float currentDistance
        {
            get
            {
                return mCurrentDistance;
            }
            set
            {
                mCurrentDistance = value;
            }
        }

        public override float minSize
        {
            get
            {
                return mMinDistance;
            }
            set
            {
                mMinDistance = value;
            }
        }
        public override float maxSize
        {
            get
            {
                return mMaxDistance;
            }
            set
            {
                mMaxDistance = value;
            }
        }

        public override void Init()
        {
            SetCameraDistance();
        }

        public PerspectiveCameraPinchService(Camera camera)
        {
            this.mCamera = camera;
        }

        public override void OnPinchBegin()
        {
        }

        public override void OnPinch(EventData eventData)
        {
            float detalDistance = eventData.deltaTwoFingerDistance;
            float preDistance = mCurrentDistance;
            mCurrentDistance -= detalDistance * mPinchRadiu;
            if (mCurrentDistance < mMinDistance && mCurrentDistance < preDistance)
            {
                mCurrentDistance = mMinDistance;
                return;
            }
            if (mCurrentDistance > mMaxDistance && mCurrentDistance > preDistance)
            {
                mCurrentDistance = mMaxDistance;
                return;
            }
            SetCameraDistance();
        }

        protected override void OnMouseScrollWheel()
        {
#if UNITY_STANDALONE || UNITY_EDITOR 
            EventData eventData = new EventData();
            if (Input.GetKey(KeyCode.LeftShift))
            {
                eventData.deltaTwoFingerDistance = Input.GetAxis("Mouse ScrollWheel") * 900f;
            }
            else
            {
                eventData.deltaTwoFingerDistance = Input.GetAxis("Mouse ScrollWheel") * 300f;
            }
            OnPinch(eventData);
#endif
        }

        void SetCameraDistance()
        {
            mCamera.transform.position = CameraController.Instance.GetCameraForwardPosition()- mCamera.transform.forward * mCurrentDistance;
        }
    }

}


using UnityEngine;
using BlueNoah.Event;

namespace BlueNoah.CameraControl
{
    public abstract class BaseCameraPinchService
    {
        protected bool mIsPinching;
        protected bool mIsPinchable = true;
#if UNITY_EDITOR
        protected float mPinchRadiu = 0.009f;
#else
        protected float mPinchRadiu = 0.09f;
#endif
        protected float mMaxPinchOver = 0.5f;
        protected Camera mCamera;
        protected float zoomFactor = 4.0f;


        public virtual void Init()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnLateUpdate()
        {
            if(Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                OnMouseScrollWheel();
                mIsPinching = true;
            }
            else
            {
                mIsPinching = false;
            }
        }

        protected abstract void OnMouseScrollWheel();

        public abstract float currentDistance
        {
            get;
            set;
        }

        public abstract float minSize
        {
            get;
            set;
        }

        public abstract float maxSize
        {
            get;
            set;
        }

        public bool IsPinchable
        {
            get
            {
                return mIsPinchable;
            }
            set
            {
                mIsPinchable = value;
            }
        }

        public bool IsPinching
        {
            get
            {
                return mIsPinching;
            }
        }

        public abstract void OnPinchBegin();

        public abstract void OnPinch(EventData eventData);

        public void OnPinchEnd()
        {
            if (mIsPinching)
            {
                mIsPinching = false;
            }
        }

        public void CancelPinch()
        {
            mIsPinching = false;
        }

    }
}
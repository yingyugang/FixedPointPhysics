/*
 神の見方カメラのコントロール
 上帝视角的相机控制
 應　彧剛（yingyugang@gmail.com）
 */
using UnityEngine;
using BlueNoah.Event;

namespace BlueNoah.CameraControl
{
    [System.Serializable]
    public class OrthographicsCameraPinchService : BaseCameraPinchService
    {

        private float mMinOrthographicSize = 2f;
        private float mMaxOrthographicSize = 5f;
        private float mTargetOrthographicSize;
        protected float mSmooth = 20f;
        private bool mMoveBack = false;
        float mCurrentDistance;
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
                return mMinOrthographicSize;
            }

            set
            {
                mMinOrthographicSize = value;
            }
        }

        public override float maxSize
        {
            get
            {
                return mMaxOrthographicSize;
            }

            set
            {
                mMaxOrthographicSize = value;
            }
        }

        public OrthographicsCameraPinchService(Camera camera)
        {
            mCamera = camera;
        }

        public override void Init()
        {

        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            if (mMoveBack)
                MoveBack();
        }

        public override void OnPinchBegin()
        {
            mTargetOrthographicSize = mCamera.orthographicSize;
        }

        public override void OnPinch(EventData eventData)
        {
            float detalDistance = eventData.deltaTwoFingerDistance;

            float sizeOver = mTargetOrthographicSize - Mathf.Clamp(mTargetOrthographicSize, mMinOrthographicSize, mMaxOrthographicSize);

            if ((detalDistance < 0 && sizeOver > 0))
            {
                mTargetOrthographicSize -= detalDistance * mPinchRadiu * Mathf.Max(0, Mathf.Cos((mMaxPinchOver - sizeOver) / 2f * Mathf.PI / (mMaxPinchOver * 2)));
            }
            else if (detalDistance > 0 && sizeOver < 0)
            {
                mTargetOrthographicSize -= detalDistance * mPinchRadiu * Mathf.Max(0, Mathf.Cos((mMaxPinchOver * 2 - sizeOver) / 2f * Mathf.PI / (mMaxPinchOver * 4)));
            }
            else
            {
                mTargetOrthographicSize -= detalDistance * mPinchRadiu;
            }

            //mTargetOrthographicSize = Mathf.Clamp(mTargetOrthographicSize, mMinOrthographicSize, mMaxOrthographicSize);
            mCamera.orthographicSize = mTargetOrthographicSize;
        }

        void MoveBack()
        {
            if (!mIsPinching)
            {
                //範囲内のサイズ
                float targetOrthographicSize = Mathf.Clamp(mCamera.orthographicSize, mMinOrthographicSize, mMaxOrthographicSize);
                //ピンチ必要なサイズ
                targetOrthographicSize = mCamera.orthographicSize - targetOrthographicSize;
                //スムース的な戻す
                mCamera.orthographicSize -= targetOrthographicSize * Time.deltaTime * mSmooth / 2f;
            }
        }

        protected override void OnMouseScrollWheel()
        {
            mTargetOrthographicSize = mCamera.orthographicSize;
            EventData eventData = new EventData();
            eventData.deltaTwoFingerDistance = Input.GetAxis("Mouse ScrollWheel") * 100f;
            OnPinch(eventData);
        }
    }
}


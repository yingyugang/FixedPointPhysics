using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace BlueNoah.Event
{
    public enum TouchType { TouchBegin = 101, Touch = 102, TouchEnd = 103, Click = 104, DoubleClick = 105, LongPressBegin = 106, LongPress = 107, LongPressEnd = 108, TwoFingerBegin = 201, TwoFinger = 202, TwoFingerEnd = 203 };

    public class TouchData
    {
        public Touch startTouch;
        public float startTime;
        public Touch touch;
        public Touch endTouch;
        public float endTime;
        public float preEndTime;
        public bool isLongPressed;
        public bool isPointerOnGameObject;
        public bool isClicked;
        public int continuousClick;
        public bool isActive;
    }

    public class EventData
    {
        // public Vector3 touchStartPos0;
        // public Vector3 touchPos0;
        // public Vector3 deltaTouchPos0;
        // public float touchStartTime0;
        // public float touchEndTime0;
        // public bool isPointerOnGameObject;
        // public Vector3 touchStartPos1;
        // public Vector3 touchPos1;
        // public Vector3 deltaTouchPos1;
        // public float touchStartTime1;
        // public float touchEndTime1;
        // public float deltaAngle;
        // public float pinchDistance;
        public float currentTwoFingerDistance;
        public float deltaTwoFingerDistance;
        // public bool isLongPressed;
        // public float preClickTime;
        public TouchData currentTouch;
        public Dictionary<int, TouchData> touchDictionary;

        public EventData()
        {
            touchDictionary = new Dictionary<int, TouchData>();
        }

        public bool StartTouch(Touch touch)
        {
            TouchData touchData;
            if (!touchDictionary.ContainsKey(touch.fingerId))
            {
                touchData = new TouchData();
                touchDictionary.Add(touch.fingerId, touchData);
            }
            touchData = touchDictionary[touch.fingerId];
            touchData.isActive = true;
            touchData.startTouch = touch;
            touchData.touch = touch;
            touch.rawPosition = touch.position;
            touchData.startTime = Time.realtimeSinceStartup;
            touchData.isPointerOnGameObject = IsPointerOverUIObject(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            this.currentTouch = touchData;
            return true;
        }

        public bool UpdateTouch(Touch touch)
        {
            if (touchDictionary.ContainsKey(touch.fingerId))
            {
                touchDictionary[touch.fingerId].touch = touch;
                this.currentTouch = touchDictionary[touch.fingerId];
                return true;
            }
            return false;
        }

        public bool EndTouch(Touch touch)
        {
            // TouchData touchData = new TouchData();
            if (touchDictionary.ContainsKey(touch.fingerId))
            {
                TouchData touchData = touchDictionary[touch.fingerId];
                // touchDictionary.Remove(touch.fingerId);]
                touchData.isActive = false;
                touchData.endTouch = touch;
                touchData.preEndTime = touchData.endTime;
                touchData.endTime = Time.realtimeSinceStartup;
                this.currentTouch = touchData;
                return true;
            }
            return false;
        }

        public TouchData GetTouchData(int fingerId)
        {
            if (touchDictionary.ContainsKey(fingerId))
            {
                return touchDictionary[fingerId];
            }
            return null;
        }

        public bool HasTouchData(int fingerId)
        {
            return touchDictionary.ContainsKey(fingerId);
        }

        bool IsPointerOverUIObject(Vector2 position)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(position.x, position.y);
            List<RaycastResult> results = new List<RaycastResult>();
            if (EventSystem.current != null)
            {
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                return results.Count > 0;
            }
            else
            {
                return false;
            }
        }
    }

    //説明：GetTouchDownなどの名前と仕組みは、Input.GetMouseBottonDown(0)と近いてる。
    public class EasyInput : SimpleSingleMonoBehaviour<EasyInput>
    {
        //Click interval max(same as ugui system)
        //Double click interval 0.3f~0.5f
        Dictionary<TouchType, List<UnityAction<EventData>>> mGlobalTouchActionDic;
        Dictionary<TouchType, List<UnityAction<EventData>>> mGlobalLateUpdateTouchActionDic;
        Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> mTouchAtionsWithFingerIdDic;
        Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> mLateUpdateTouchActionsWithFingerIdDic;
        EventData mEventData;
        EventData mLateEventData;
        BaseInputService mBaseInputService;
        protected override void Awake()
        {
            base.Awake();
            if (mGlobalTouchActionDic == null)
            {
                Init();
            }
        }
        void Init()
        {
            mEventData = new EventData();
            mLateEventData = new EventData();
            mTouchAtionsWithFingerIdDic = new Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>>();
            mLateUpdateTouchActionsWithFingerIdDic = new Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>>();
            mGlobalTouchActionDic = new Dictionary<TouchType, List<UnityAction<EventData>>>();
            mGlobalTouchActionDic.Add(TouchType.TouchBegin, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.TouchEnd, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.Touch, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.Click, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.DoubleClick, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.LongPressBegin, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.LongPress, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.LongPressEnd, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.TwoFingerBegin, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.TwoFinger, new List<UnityAction<EventData>>());
            mGlobalTouchActionDic.Add(TouchType.TwoFingerEnd, new List<UnityAction<EventData>>());

            mGlobalLateUpdateTouchActionDic = new Dictionary<TouchType, List<UnityAction<EventData>>>();
            mGlobalLateUpdateTouchActionDic.Add(TouchType.TouchBegin, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.TouchEnd, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.Touch, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.Click, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.DoubleClick, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.LongPressBegin, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.LongPress, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.LongPressEnd, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.TwoFingerBegin, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.TwoFinger, new List<UnityAction<EventData>>());
            mGlobalLateUpdateTouchActionDic.Add(TouchType.TwoFingerEnd, new List<UnityAction<EventData>>());
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            mBaseInputService = new StandardInputService();
#else
            mBaseInputService = new MobileInputService();
#endif
            mBaseInputService.globalTouchActionsDic = this.mGlobalTouchActionDic;
            mBaseInputService.globalLateUpdateTouchActionsDic = this.mGlobalLateUpdateTouchActionDic;
            mBaseInputService.touchAtionsWithFingerIdDic = this.mTouchAtionsWithFingerIdDic;
            mBaseInputService.lateUpdateTouchActionsWithFingerIdDic = this.mLateUpdateTouchActionsWithFingerIdDic;
            mBaseInputService.eventData = this.mEventData;
            mBaseInputService.lateEventData = this.mLateEventData;
        }
        public void RemoveAllGlobalListener(TouchType touchType)
        {
            if (mGlobalTouchActionDic.ContainsKey(touchType))
            {
                mGlobalTouchActionDic.Remove(touchType);
            }
            if (mGlobalLateUpdateTouchActionDic.ContainsKey(touchType))
            {
                mGlobalLateUpdateTouchActionDic.Remove(touchType);
            }
        }
        public void RemoveAllListener(int fingerId, TouchType touchType)
        {
            if (mTouchAtionsWithFingerIdDic.ContainsKey(fingerId))
            {
                if (mTouchAtionsWithFingerIdDic[fingerId].ContainsKey(touchType))
                {
                    mTouchAtionsWithFingerIdDic[fingerId].Remove(touchType);
                }
            }
        }
        public void AddGlobalListener(TouchType touchType, UnityAction<EventData> unityAction)
        {
            if (mGlobalTouchActionDic == null)
                Init();
            mGlobalTouchActionDic[touchType].Add(unityAction);
        }
        public void AddGlobalLateUpdateListener(TouchType touchType, UnityAction<EventData> unityAction)
        {
            if (mGlobalLateUpdateTouchActionDic == null)
                Init();
            mGlobalLateUpdateTouchActionDic[touchType].Add(unityAction);
        }
        public void AddListener(TouchType touchType, int index, UnityAction<EventData> unityAction)
        {
            if (mTouchAtionsWithFingerIdDic == null)
                Init();
            if (touchType == TouchType.TwoFingerBegin || touchType == TouchType.TwoFinger || touchType == TouchType.TwoFingerEnd)
            {
                Debug.LogError(touchType + " is not support. Use global action to instead.");
                return;
            }
            if (!mTouchAtionsWithFingerIdDic.ContainsKey(index))
            {
                mTouchAtionsWithFingerIdDic.Add(index, new Dictionary<TouchType, List<UnityAction<EventData>>>());
            }
            if (!mTouchAtionsWithFingerIdDic[index].ContainsKey(touchType))
            {
                mTouchAtionsWithFingerIdDic[index].Add(touchType, new List<UnityAction<EventData>>());
            }
            mTouchAtionsWithFingerIdDic[index][touchType].Add(unityAction);
        }
        public void AddLateUpdateListener(TouchType touchType, int index, UnityAction<EventData> unityAction)
        {
            if (mLateUpdateTouchActionsWithFingerIdDic == null)
                Init();
            if (touchType == TouchType.TwoFingerBegin || touchType == TouchType.TwoFinger || touchType == TouchType.TwoFingerEnd)
            {
                Debug.LogError(touchType + " is not support. Use global action to instead.");
                return;
            }
            if (!mLateUpdateTouchActionsWithFingerIdDic.ContainsKey(index))
            {
                mLateUpdateTouchActionsWithFingerIdDic.Add(index, new Dictionary<TouchType, List<UnityAction<EventData>>>());
            }
            if (!mLateUpdateTouchActionsWithFingerIdDic[index].ContainsKey(touchType))
            {
                mLateUpdateTouchActionsWithFingerIdDic[index].Add(touchType, new List<UnityAction<EventData>>());
            }
            mLateUpdateTouchActionsWithFingerIdDic[index][touchType].Add(unityAction);
        }
        public Vector3 GetTouchPosition(int index)
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            if (index == 0)
            {
                return Input.mousePosition;
            }
            else
            {
                return Vector3.zero;
            }
#else
            return (Vector3)Input.touches[index].position;
#endif
        }
        bool GetTwoFingerBegin()
        {
            bool isTwoFingerDown = false;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            if ((Input.GetKeyDown(KeyCode.Q) && Input.GetMouseButton(0)) || (Input.GetKey(KeyCode.Q) && Input.GetMouseButtonDown(0))
               || (Input.GetKeyDown(KeyCode.Q) && Input.GetMouseButtonDown(0)))
            {
                isTwoFingerDown = true;

            }
#else
                    if ((Input.touches.Length == 2 && Input.touches[1].phase == TouchPhase.Began && Input.touches[0].phase != TouchPhase.Ended && Input.touches[0].phase != TouchPhase.Canceled) 
                        || (Input.touches.Length == 2 && Input.touches[0].phase == TouchPhase.Began && Input.touches[1].phase != TouchPhase.Ended && Input.touches[1].phase != TouchPhase.Canceled))
                    {
                        isTwoFingerDown =  true;
                    }
#endif
            return isTwoFingerDown;
        }
        bool GetTwoFinger()
        {
            bool isTwoFinger = false;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            if (Input.GetKey(KeyCode.Q) && Input.GetMouseButton(0))
            {
                isTwoFinger = true;
            }
#else
                    if(Input.touches.Length == 2 && (Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary || Input.touches[1].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Stationary)){
                        isTwoFinger = true;
                    }
#endif
            return isTwoFinger;
        }
        bool GetTwoFingerEnd()
        {
            bool isTwoFingerUp = false;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            if ((Input.GetKeyUp(KeyCode.Q) && Input.GetMouseButton(0)) || (Input.GetKey(KeyCode.Q) && Input.GetMouseButtonUp(0))
                || (Input.GetKeyUp(KeyCode.Q) && Input.GetMouseButtonUp(0)))
            {
                isTwoFingerUp = true;
            }
#else
                    if ((Input.touches.Length == 2 && (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)) 
                        || (Input.touches.Length == 2 && (Input.touches[1].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Canceled)))
                    {
                        isTwoFingerUp =  true;
                    }
#endif
            return isTwoFingerUp;
        }
        void OnActions(List<UnityAction<EventData>> unityActions, EventData eventData)
        {
            for (int i = 0; i < unityActions.Count; i++)
            {
                if (unityActions[i] != null)
                    unityActions[i](eventData);
            }
        }
        void Update()
        {
            mBaseInputService.OnUpdate();
            // OnUpdate(mGlobalTouchActionDic, mEventData);
        }
        void LateUpdate()
        {
            mBaseInputService.OnLateUpdate();
            // OnUpdate(mGlobalLateUpdateTouchActionDic, mLateEventData);
        }

        // void OnUpdate(Dictionary<TouchType, List<UnityAction<EventData>>> actionDic, EventData eventData)
        // {
        //     mBaseInputService.GetTouchDown(actionDic, eventData);
        //     mBaseInputService.GetTouch(actionDic, eventData);
        //     mBaseInputService.GetTouchUp(actionDic, eventData);
        // }

        bool CheckOverGameObject()
        {
#if UNITY_EDITOR
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
#else
            if (EventSystem.current != null && Input.touchCount > 0)
            {

                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return true;
            }
#endif
            return false;
        }

        private void OnDestroy()
        {
            foreach (TouchType touchType in mGlobalTouchActionDic.Keys)
            {
                mGlobalTouchActionDic[touchType].Clear();
            }
            foreach (TouchType touchType in mGlobalLateUpdateTouchActionDic.Keys)
            {
                mGlobalLateUpdateTouchActionDic[touchType].Clear();
            }
        }

        // void OnTwoFingerBegin(EventData eventData)
        // {
        //     eventData.touchPos0 = GetTouchPosition(0);
        //     eventData.touchPos1 = GetTouchPosition(1);
        //     eventData.touchStartPos1 = GetTouchPosition(1);
        //     eventData.pinchDistance = Vector3.Distance(eventData.touchPos0, eventData.touchPos1);
        // }

        // void OnTwoFinger(EventData eventData)
        // {
        //     eventData.deltaTouchPos0 = GetTouchPosition(0) - eventData.touchPos0;
        //     eventData.deltaTouchPos1 = GetTouchPosition(1) - eventData.touchPos1;
        //     eventData.deltaAngle = Vector3.SignedAngle(GetTouchPosition(1) - GetTouchPosition(0), eventData.touchPos1 - eventData.touchPos0, new Vector3(0, 0, 1));
        //     eventData.touchPos0 = GetTouchPosition(0);
        //     eventData.touchPos1 = GetTouchPosition(1);
        //     float currentDistance = Vector3.Distance(eventData.touchPos0, eventData.touchPos1);
        //     eventData.deltaPinchDistance = currentDistance - eventData.pinchDistance;
        //     eventData.pinchDistance = currentDistance;
        // }
        // void OnTwoFingerEnd()
        // {

        // }
    }
}




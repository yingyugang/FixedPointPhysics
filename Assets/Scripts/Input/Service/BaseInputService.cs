using System.Collections;
using System.Collections.Generic;
using BlueNoah.Event;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Event
{
    public abstract class BaseInputService
    {

        const float MIN_DOUBLE_CLICK_INTERVAL = 0.3f;
        const float MIN_LONGPRESS_DURATION = 1f;
        const float MIN_LONGPRESS_DISTANCE = 3;
        const float MIN_CLICK_DISTANCE = 3;
        public Dictionary<TouchType, List<UnityAction<EventData>>> globalTouchActionsDic;
        public Dictionary<TouchType, List<UnityAction<EventData>>> globalLateUpdateTouchActionsDic;
        public Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> touchAtionsWithFingerIdDic;
        public Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> lateUpdateTouchActionsWithFingerIdDic;
        public EventData eventData;
        public EventData lateEventData;
        public void OnUpdate()
        {
            CheckTouchDown(globalTouchActionsDic, touchAtionsWithFingerIdDic, eventData);

            CheckTouch(globalTouchActionsDic, touchAtionsWithFingerIdDic, eventData);

            CheckTouchUp(globalTouchActionsDic, touchAtionsWithFingerIdDic, eventData);
        }
        public void OnLateUpdate()
        {
            CheckTouchDown(globalLateUpdateTouchActionsDic, lateUpdateTouchActionsWithFingerIdDic, lateEventData);

            CheckTouch(globalLateUpdateTouchActionsDic, lateUpdateTouchActionsWithFingerIdDic, lateEventData);

            CheckTouchUp(globalLateUpdateTouchActionsDic, lateUpdateTouchActionsWithFingerIdDic, lateEventData);
        }
        protected abstract void CheckTouchDown(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData);
        protected void OnTouchDown(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            OnActions(globalActionDic, actionDic, eventData, eventData.currentTouch.startTouch.fingerId, TouchType.TouchBegin);
            OnTwoFingerBegin(globalActionDic, actionDic, eventData);
        }
        protected abstract void CheckTouch(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData);
        protected void OnTouch(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            OnActions(globalActionDic, actionDic, eventData, eventData.currentTouch.touch.fingerId, TouchType.Touch);
            OnLongPress(globalActionDic, actionDic, eventData, eventData.currentTouch.touch.fingerId);
            OnLongPressBegin(globalActionDic, actionDic, eventData, eventData.currentTouch.touch.fingerId);
            OnTwoFinger(globalActionDic, actionDic, eventData);
        }

        protected abstract void CheckTouchUp(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData);

        protected void OnTouchUp(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            OnActions(globalActionDic, actionDic, eventData, eventData.currentTouch.endTouch.fingerId, TouchType.TouchEnd);
            OnClick(globalActionDic, actionDic, eventData, eventData.currentTouch.endTouch.fingerId);
            OnLongPressEnd(globalActionDic, actionDic, eventData, eventData.currentTouch.endTouch.fingerId);
            OnTwoFingerEnd(globalActionDic, actionDic, eventData);
        }

        protected void OnActions(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData, int fingerId, TouchType touchType)
        {
            if (actionDic.ContainsKey(fingerId) && actionDic[fingerId].ContainsKey(touchType))
            {
                OnActions(actionDic[fingerId][touchType], eventData);
            }
            if (globalActionDic.ContainsKey(touchType))
            {
                OnActions(globalActionDic[touchType], eventData);
            }
        }
        protected void OnClick(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData, int fingerId)
        {
            TouchData touchData = eventData.currentTouch;
            float distance = Vector3.Distance(touchData.startTouch.position, touchData.endTouch.position);
            if (distance / Screen.dpi * 25.4 < MIN_CLICK_DISTANCE)
            {
                OnActions(globalActionDic, actionDic, eventData, fingerId, TouchType.Click);
                OnDoubleClick(globalActionDic, actionDic, eventData, fingerId);
            }
        }
        protected void OnDoubleClick(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData, int fingerId)
        {
            TouchData touchData = eventData.currentTouch;
            if (Time.realtimeSinceStartup - touchData.preEndTime < MIN_DOUBLE_CLICK_INTERVAL)
            {
                touchData.continuousClick++;
                if (touchData.continuousClick == 1)
                {
                    OnActions(globalActionDic, actionDic, eventData, fingerId, TouchType.DoubleClick);
                }
            }
            else
            {
                touchData.continuousClick = 0;
            }
        }
        protected void OnLongPressBegin(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData, int fingerId)
        {
            TouchData touchData = eventData.currentTouch;
            if (!touchData.isLongPressed && Time.realtimeSinceStartup - touchData.startTime > MIN_LONGPRESS_DURATION)
            {
                float distance = Vector3.Distance(touchData.startTouch.position, touchData.touch.position);
                if (distance / Screen.dpi * 25.4 < MIN_LONGPRESS_DISTANCE)
                {
                    OnActions(globalActionDic, actionDic, eventData, fingerId, TouchType.LongPressBegin);
                    touchData.isLongPressed = true;
                }
            }
        }
        protected void OnLongPress(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData, int fingerId)
        {
            TouchData touchData = eventData.currentTouch;
            if (touchData.isLongPressed)
            {
                OnActions(globalActionDic, actionDic, eventData, fingerId, TouchType.LongPress);
            }
        }
        protected void OnLongPressEnd(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData, int fingerId)
        {
            TouchData touchData = eventData.currentTouch;
            if (touchData.isLongPressed)
            {
                OnActions(globalActionDic, actionDic, eventData, fingerId, TouchType.LongPressEnd);
                touchData.isLongPressed = false;
            }
        }
        protected void OnActions(List<UnityAction<EventData>> unityActions, EventData eventData)
        {
            for (int i = 0; i < unityActions.Count; i++)
            {
                if (unityActions[i] != null)
                    unityActions[i](eventData);
            }
        }
        //全局Event;
        protected void OnTwoFingerBegin(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            if ((eventData.HasTouchData(0) && eventData.GetTouchData(0).isActive) && (eventData.HasTouchData(1) && eventData.GetTouchData(1).isActive))
            {
                if (eventData.GetTouchData(0) == eventData.currentTouch || eventData.GetTouchData(1) == eventData.currentTouch)
                {
                    //-1：run it when global only.
                    OnActions(globalActionDic, actionDic, eventData, -1, TouchType.TwoFingerBegin);
                    Vector3 pos0 = eventData.GetTouchData(0).startTouch.position;
                    Vector3 pos1 = eventData.GetTouchData(1).startTouch.position;
                    eventData.currentTwoFingerDistance = Vector3.Distance(pos0, pos1);
                    eventData.deltaTwoFingerDistance = 0;
                }
            }
        }
        //全局Event;
        protected void OnTwoFinger(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            if ((eventData.HasTouchData(0) && eventData.GetTouchData(0).isActive) && (eventData.HasTouchData(1) && eventData.GetTouchData(1).isActive))
            {
                //-1：run it when global only.
                OnActions(globalActionDic, actionDic, eventData, -1, TouchType.TwoFinger);
                Vector3 pos0 = eventData.GetTouchData(0).touch.position;
                Vector3 pos1 = eventData.GetTouchData(1).touch.position;
                float currentTwoFingerDistance = Vector3.Distance(pos0, pos1);
                eventData.deltaTwoFingerDistance = currentTwoFingerDistance - eventData.currentTwoFingerDistance;
                eventData.currentTwoFingerDistance = currentTwoFingerDistance;
            }
        }
        //全局Event;
        protected void OnTwoFingerEnd(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            if ((eventData.HasTouchData(0) && (eventData.GetTouchData(0) == eventData.currentTouch || eventData.GetTouchData(0).isActive))
                        && (eventData.HasTouchData(1) && (eventData.GetTouchData(1) == eventData.currentTouch || eventData.GetTouchData(1).isActive)))
            {
                //-1：run it when global only.
                OnActions(globalActionDic, actionDic, eventData, -1, TouchType.TwoFingerEnd);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using BlueNoah.Event;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Event
{
    public class MobileInputService : BaseInputService
    {
        protected override void CheckTouchDown(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    if (eventData.StartTouch(touch))
                    {
                        OnTouchDown(globalActionDic, actionDic, eventData);
                    }
                }
            }
        }
        protected override void CheckTouch(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (eventData.UpdateTouch(touch))
                    {
                        OnTouch(globalActionDic, actionDic, eventData);
                    }
                }
            }
        }
        protected override void CheckTouchUp(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                {
                    if (eventData.EndTouch(touch))
                    {
                        OnTouchUp(globalActionDic, actionDic, eventData);
                    }
                }
            }
        }
    }
}

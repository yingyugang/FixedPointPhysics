using System.Collections;
using System.Collections.Generic;
using BlueNoah.Event;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Event
{
    public class StandardInputService : BaseInputService
    {

        Dictionary<int, Touch> touches = new Dictionary<int, Touch>();
        Dictionary<int, Touch> endTouches = new Dictionary<int, Touch>();

        protected override void CheckTouchDown(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!touches.ContainsKey(0))
                {
                    touches.Add(0, CreateTouchByMouseEvent(0));
                }
                else
                {
                    touches[0] = CreateTouchByMouseEvent(0);
                }
                if (eventData.StartTouch(touches[0]))
                {
                    OnTouchDown(globalActionDic, actionDic, eventData);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (!touches.ContainsKey(1))
                {
                    touches.Add(1, CreateTouchByMouseEvent(1));
                }
                else
                {
                    touches[1] = CreateTouchByMouseEvent(1);
                }
                if (eventData.StartTouch(touches[1]))
                {
                    OnTouchDown(globalActionDic, actionDic, eventData);
                }
            }
        }

        protected override void CheckTouch(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            if (Input.GetMouseButton(0))
            {
                if (touches.ContainsKey(0))
                {
                    Touch touch = touches[0];
                    touch.position = Input.mousePosition;
                    touch.deltaPosition = (Vector2)Input.mousePosition - eventData.currentTouch.touch.position;
                    if (eventData.UpdateTouch(touch))
                    {
                        OnTouch(globalActionDic, actionDic, eventData);
                    }
                }
            }
            if (Input.GetMouseButton(1))
            {
                Touch touch = touches[1];
                touch.deltaPosition = (Vector2)Input.mousePosition - eventData.currentTouch.touch.position;
                if (eventData.UpdateTouch(touch))
                {
                    OnTouch(globalActionDic, actionDic, eventData);
                }
            }
        }
        protected override void CheckTouchUp(Dictionary<TouchType, List<UnityAction<EventData>>> globalActionDic, Dictionary<int, Dictionary<TouchType, List<UnityAction<EventData>>>> actionDic, EventData eventData)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!endTouches.ContainsKey(0))
                {
                    endTouches.Add(0, CreateTouchByMouseEvent(0));
                }
                else
                {
                    endTouches[0] = CreateTouchByMouseEvent(0);
                }
                if (eventData.EndTouch(endTouches[0]))
                {
                    OnTouchUp(globalActionDic, actionDic, eventData);
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                if (!endTouches.ContainsKey(1))
                {
                    endTouches.Add(1, CreateTouchByMouseEvent(1));
                }
                else
                {
                    endTouches[1] = CreateTouchByMouseEvent(1);
                }
                if (eventData.EndTouch(endTouches[1]))
                {
                    OnTouchUp(globalActionDic, actionDic, eventData);
                }
            }
        }
        Touch CreateTouchByMouseEvent(int mouseButtonIndex)
        {
            Touch touch = new Touch();
            touch.fingerId = mouseButtonIndex;
            touch.deltaTime = Time.deltaTime;
            touch.position = Input.mousePosition;
            touch.rawPosition = Input.mousePosition;
            return touch;
        }
    }
}

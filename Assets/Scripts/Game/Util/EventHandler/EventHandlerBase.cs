/*
 * UI 버튼 클릭 및 기타 이벤트를 송/수신 처리하기 위한 코드입니다.
 * 송신측에서는 EventHandlerBase을 상속받아 DoEvent 처리하고,
 * 수신측에서는 송신하는 객체 인스턴스를 받아와 해당 객체에 Bind하여 처리하는 방식입니다.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Util.EventHandler
{
    public class EventHandlerBase : MonoBehaviour
    {
        private Dictionary<(int, Type), Action<object[]>> eventDict;

        public void DoEvent<T>(T key, params object[] objects)
        {
            if (eventDict == null)
                return;
        
            var eventKey = (key.GetHashCode(), typeof(T));
        
            if (eventDict.TryGetValue(eventKey, out var action))
                action?.Invoke(objects);
        }

        public void BindEvent<T>(T key, Action<object[]> action)
        {
            eventDict ??= new Dictionary<(int, Type), Action<object[]>>();

            var eventKey = (key.GetHashCode(), typeof(T));
            
            // 키가 중복으로 들어올 경우 의도된 동작이 아니므로 무시합니다.
            if(!eventDict.ContainsKey(eventKey))
                eventDict.Add(eventKey, action);
        }

        public bool RemoveEvent<T>(T key)
        {
            if (eventDict == null)
                return false;
        
            var eventKey = (key.GetHashCode(), typeof(T));
       
            bool ret = eventDict.Remove(eventKey);
        
            if (eventDict.Count == 0)
                eventDict = null;

            return ret;
        }
    
        public Action<object[]> FindEvent<T>(T key)
        {
            if (eventDict == null)
                return null;

            var eventKey = (key.GetHashCode(), typeof(T));
            
            return eventDict.GetValueOrDefault(eventKey);
        }
        
        public void RemoveAllEvents()
        {
            eventDict = null;
        }

        protected virtual void OnDestroy()
        {
            RemoveAllEvents();
        }
    }
}
/*
 * static 클래스에서도 코루틴을 사용할 수 있도록 만든 클래스
 */

using System;
using System.Collections;
using UnityEngine;

namespace Game.Manager
{
    public sealed class CoroutineTaskManager : MonoBehaviour
    {
        private static CoroutineTaskManager instance = null;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        
        public static Coroutine AddTask(IEnumerator routine)
        {
            try
            {
                if (instance && instance.gameObject.activeSelf)
                    return instance.StartCoroutine(routine);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.StackTrace);
                Debug.LogError(e);
            }
            return null;
        }

        public static void ClearTasks()
        {
            if (instance)
                instance.StopAllCoroutines();
        }

        public static void RemoveTask(Coroutine co)
        {
            try
            {
                if (instance)
                    instance.StopCoroutine(co);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.StackTrace);
                Debug.LogError(e);
            }
        }
    }    
}
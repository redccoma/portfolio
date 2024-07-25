/*
 *  UnityEngine.SceneManagement.SceneManager Warpping 매니저 클래스
 */
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    using Manager = UnityEngine.SceneManagement.SceneManager;
    
    public static class SceneManager
    {
        public enum SceneType
        {
            Entry,
            Loading,
            Lobby,
            Stage1,
            Stage2,
            Stage3
        }
        
        public static SceneType NextScene { get; private set; }
        public static Action<float> OnLoadingProgress;
        
        public static void LoadScene(SceneType scene)
        {
            NextScene = scene;
            
            // 로딩씬 Additive로 로드.
            CoroutineTaskManager.AddTask(LoadSceneAsync(GetSceneName(SceneType.Loading)));
        }
        
        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = Manager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return asyncLoad;
            
            OnLoadingProgress?.Invoke(0f);
            
            AsyncOperation loadOperation = Manager.LoadSceneAsync(GetSceneName(NextScene));
            loadOperation.allowSceneActivation = false;
            
            while (!loadOperation.isDone)
            {
                OnLoadingProgress?.Invoke(Mathf.Clamp01(loadOperation.progress / 0.9f));
                yield return null;
                
                // 로딩이 거의 완료되면 씬 활성화
                if (loadOperation.progress >= 0.9f)
                {
                    OnLoadingProgress?.Invoke(Mathf.Clamp01(loadOperation.progress / 0.9f));
                    yield return new WaitForSeconds(1); // 잠시 대기
                    loadOperation.allowSceneActivation = true;
                }
            }
        }

        private static string GetSceneName(SceneType type)
        {
            switch (type)
            {
                case SceneType.Entry:
                    return "Game_Entry";
                case SceneType.Loading:
                    return "Game_Loading";
                case SceneType.Lobby:
                    return "Game_Lobby";
                case SceneType.Stage1:
                    return "Game_Stage1";
                case SceneType.Stage2:
                    return "Game_Stage2";
                case SceneType.Stage3:
                    return "Game_Stage3";
                default:
                    return "";
            }
        }
    }
}

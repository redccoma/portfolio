/*
 *  UnityEngine.SceneManagement.SceneManager Warpping 매니저 클래스
 *  Loading 씬과 유기적으로 커플링되어 있다.
 */
using System;
using System.Collections;
using Game.GameLoading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    using Manager = UnityEngine.SceneManagement.SceneManager;
    
    public static class SceneManager
    {
        // Start 씬은 미포함.
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

        public static void DirectLoadScene(SceneType type)
        {
            Manager.LoadScene(GetSceneName(type));
        }
        
        public static void LoadScene(SceneType currentScene, SceneType nextScene)
        {
            NextScene = nextScene;
            
            string current = GetSceneName(currentScene);
            string next = GetSceneName(nextScene);
            
            // 로딩씬 Additive로 로드.
            CoroutineTaskManager.AddTask(LoadSceneAsync(current, next));
        }
        
        private static IEnumerator LoadSceneAsync(string current, string next)
        {
            // 로딩씬 Additive로 로드.
            string loading = GetSceneName(SceneType.Loading);
            AsyncOperation asyncLoad = Manager.LoadSceneAsync(loading, LoadSceneMode.Additive);
            yield return asyncLoad;
            
            // 로딩중 GC 호출
            GC.Collect();
            
            // Loading 씬에서 애니메이션 시작완료까지 대기.
            bool isAnimationComplete = false;
            LoadingScene.OnAnimationComplete = () =>
            {
                isAnimationComplete = true;
            };
            yield return new WaitUntil(()=> isAnimationComplete);
            
            // 기존 씬 remove
            AsyncOperation unloadOperation = Manager.UnloadSceneAsync(current);
            yield return unloadOperation;

            // 프로그래스바 초기화
            OnLoadingProgress?.Invoke(0f);
                            
            AsyncOperation loadOperation = Manager.LoadSceneAsync(next, LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = false;
            
            while (!loadOperation.isDone)
            {
                // 0.1은 씬을 실제 활성화하고 초기화하는 과정을 나타내므로 max 0.9로 비교.
                float _progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                
                OnLoadingProgress?.Invoke(_progress);
                yield return null;
                
                if (_progress >= 1f)
                    break;
            }
            
            // 다음 씬을 보여주기위한 타이밍 대기.
            LoadingScene.OnShowNextScene = () =>
            {
                loadOperation.allowSceneActivation = true;
            };

            LoadingScene.OnLoadingComplete -= UnloadLoadingScene;
            LoadingScene.OnLoadingComplete += UnloadLoadingScene;
        }
        
        private static void UnloadLoadingScene()
        {
            Manager.UnloadSceneAsync(GetSceneName(SceneType.Loading));
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

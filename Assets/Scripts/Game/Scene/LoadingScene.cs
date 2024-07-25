/*
 * Loading 씬은 진입후 다음 씬 로딩전까지 화면전환 및 프로그래스바를 담당합니다.
 */

using UnityEngine;
using UnityEngine.UI;

namespace Game.Scene
{
    using SceneManager = Game.Manager.SceneManager;
    
    public class LoadingScene : MonoBehaviour
    {
        public Slider slider; 
            
        private void Awake()
        {
            SceneManager.OnLoadingProgress -= OnLoadingProgress;
            SceneManager.OnLoadingProgress += OnLoadingProgress;
        }
        
        private void OnLoadingProgress(float progress)
        {
            slider.value = progress;
        }

        private void OnDestroy()
        {
            SceneManager.OnLoadingProgress -= OnLoadingProgress;
        }
    }
}
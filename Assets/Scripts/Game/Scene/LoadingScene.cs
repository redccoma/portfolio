/*
 * Loading 씬은 진입후 SceneManager가 가지고 있는 NextScene을 확인하여 해당 씬을 로드하는 역할을 한다.
 */

using System;
using System.Collections;
using System.Collections.Generic;
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
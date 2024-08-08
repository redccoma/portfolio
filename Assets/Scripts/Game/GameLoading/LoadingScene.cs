/*
 * Loading 씬은 진입후 다음 씬 로딩전까지 화면전환 및 프로그래스바를 담당합니다.
 */
using System;
using System.Collections;
using Game.UI;
using Game.Util.ScriptFieldSetup;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameLoading
{
    using SceneManager = Game.Manager.SceneManager;
    
    public class LoadingScene : MonoBehaviour
    {
        private const float FadeDuration = 1f;
        
        // 로딩씬 진입후 Fadeinout 애니메이션 종료알림. 
        public static Action OnAnimationComplete;
        // 다음 씬 등장 가능 알림.
        public static Action OnShowNextScene;
        // 로딩씬의 모든 처리가 종료되었음을 알림.
        public static Action OnLoadingComplete;
        
        [SerializeField]
        private CanvasGroup canvasGroup;
        
        [SerializeField] 
        private GameObject backgroundObj;

        [SerializeField] 
        private CanvasGroup sliderCanvasGroup;
        
        [SerializeField]
        private Slider slider;
            
        private void Awake()
        {
            SceneManager.OnLoadingProgress -= OnLoadingProgress;
            SceneManager.OnLoadingProgress += OnLoadingProgress;
        }

        private IEnumerator Start()
        {
            backgroundObj.SetActive(false);
            canvasGroup.alpha = 0;
            sliderCanvasGroup.alpha = 0;

            yield return Fade(canvasGroup, true, FadeDuration);
            
            backgroundObj.SetActive(true);
            
            yield return Fade(sliderCanvasGroup, true, FadeDuration);
            
            OnAnimationComplete?.Invoke();
        }

        private IEnumerator NextScene()
        {
            yield return Fade(sliderCanvasGroup, false, FadeDuration);
            
            canvasGroup.alpha = 1;
            backgroundObj.SetActive(false);
            
            OnShowNextScene?.Invoke();

            yield return Fade(canvasGroup, false, FadeDuration);
            
            OnLoadingComplete?.Invoke();
        }

        private IEnumerator Fade(CanvasGroup cg, bool fadeIn, float duration)
        {
            float startAlpha = fadeIn ? 0f : 1f;
            float endAlpha = fadeIn ? 1f : 0f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                cg.alpha = newAlpha;
                yield return null;
            }

            canvasGroup.alpha = endAlpha; // 정확한 최종값 설정
        }
        
        private void OnLoadingProgress(float progress)
        {
            slider.value = progress;

            if (progress >= 1f)
            {
                StartCoroutine(NextScene());
            }
        }

        private void OnDestroy()
        {
            SceneManager.OnLoadingProgress -= OnLoadingProgress;
        }
#if UNITY_EDITOR
        // 코드를 작성할때 의도된대로 필드에 원하는 데이터를 할당하도록 합니다.
        [FieldSetupButton("Field Setup")]
        private void Setup()
        {
            Transform canvas = GameObject.Find("Canvas").transform;
            
            canvasGroup = canvas.FindDeepChild("CanvasGroup").GetComponent<CanvasGroup>();
            backgroundObj = canvas.FindDeepChild("Background").gameObject;
            sliderCanvasGroup = canvas.FindDeepChild("LoadingGroup").GetComponent<CanvasGroup>(); 
            slider = canvas.FindDeepChild("Slider").GetComponent<Slider>();
        }
#endif
    }
}
/*
 * UISample의 Canvas에 attached 되었습니다.
 * 개발해상도(Canvas Scaler에 입력된 값)를 기준으로 match를 조정합니다.
 */
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UISample
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScalerAdjust : MonoBehaviour
    {
        private CanvasScaler canvasScaler;
        private float referenceAspectRatio;
        private Vector2Int lastResolution;

        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
        }

        private void OnEnable()
        {
            UpdateReferenceAspectRatio();
            lastResolution = GetCurrentResolution();
            AdjustCanvasScaler();

    #if UNITY_EDITOR
            EditorApplication.update += CheckResolutionInEditor;
    #endif
        }

        private void OnDisable()
        {
    #if UNITY_EDITOR
            EditorApplication.update -= CheckResolutionInEditor;
    #endif
        }

        private void Update()
        {
            Vector2Int currentResolution = GetCurrentResolution();
            if (currentResolution != lastResolution)
            {
                lastResolution = currentResolution;
                AdjustCanvasScaler();
            }
        }

        private Vector2Int GetCurrentResolution()
        {
    #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return GetMainGameViewSize();
            }
    #endif
            return new Vector2Int(Screen.width, Screen.height);
        }

    #if UNITY_EDITOR
        private void CheckResolutionInEditor()
        {
            if (!Application.isPlaying)
            {
                Vector2Int currentResolution = GetMainGameViewSize();
                if (currentResolution != lastResolution)
                {
                    lastResolution = currentResolution;
                    AdjustCanvasScaler();
                }
            }
        }

        // UnityEditor.GameView의 GetSizeOfMainGameView()를 호출하여 현재 GameView의 해상도를 가져옵니다.
        private Vector2Int GetMainGameViewSize()
        {
            // 클래스의 전체 이름과 어셈블리 이름
            System.Type t = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            
            // private, static GetSizeOfMainGameView 함수 정보를 가져옵니다.
            System.Reflection.MethodInfo getSizeOfMainGameView = t.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            
            // 객체 인스턴스가 필요없고, 전달할 매개변수가 없음.
            System.Object res = getSizeOfMainGameView.Invoke(null, null);
            
            // 실제코드는
            // internal static Vector2 GetSizeOfMainGameView() => PlayModeView.GetMainPlayModeViewTargetSize() 이므로 반환값을 Vector2로 변환
            Vector2 result = (Vector2)res;
            
            return Vector2Int.RoundToInt(result);
        }
    #endif

        private void UpdateReferenceAspectRatio()
        {
            referenceAspectRatio = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;
        }

        private void AdjustCanvasScaler()
        {
            UpdateReferenceAspectRatio();
            float currentAspectRatio = lastResolution.x / (float)lastResolution.y;

            float targetMatch;
            if (currentAspectRatio > referenceAspectRatio)
            {
                // 현재 화면이 기준보다 더 와이드한 경우
                targetMatch = 0; // width에 맞춤
            }
            else if (currentAspectRatio < referenceAspectRatio)
            {
                // 현재 화면이 기준보다 더 좁은 경우
                targetMatch = 1; // height에 맞춤
            }
            else
            {
                // 현재 화면과 기준의 비율이 같은 경우
                targetMatch = canvasScaler.matchWidthOrHeight;
            }

            canvasScaler.matchWidthOrHeight = targetMatch;

            Debug.Log($"화면 종횡비 조정: 현재 {currentAspectRatio:F2}, 기준 {referenceAspectRatio:F2}, 새로운 match 값: {canvasScaler.matchWidthOrHeight:F2}");
        }
    }
}
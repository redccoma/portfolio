/*
 * FieldSetupButton 어트리뷰트를 사용한 함수를 리플렉션을 사용해 인스펙터에 버튼을 만들고 실행하도록 합니다.
 */

using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Game.Util.ScriptFieldSetup
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class GenericButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            // 현재 활성화된 컴포넌트 가져오기.
            MonoBehaviour targetMB = (MonoBehaviour)target;

            // 전체 함수정보.
            MethodInfo[] methods = targetMB.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        
            foreach (MethodInfo method in methods)
            {
                // 함수의 어트리뷰트 접근.
                FieldSetupButtonAttribute editorButtonAttribute = method.GetCustomAttribute<FieldSetupButtonAttribute>();
                if (editorButtonAttribute != null)
                {
                    string buttonName = string.IsNullOrEmpty(editorButtonAttribute.Name) ? method.Name : editorButtonAttribute.Name;
                
                    if (GUILayout.Button(buttonName))
                    {
                        method.Invoke(targetMB, null);
                    }
                }
            }
        }
    }    
}
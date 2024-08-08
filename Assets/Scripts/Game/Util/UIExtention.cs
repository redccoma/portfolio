/*
 * ui관련 유틸성 코드
 */

using UnityEngine;

namespace Game.UI
{
    public static class UIExtension
    {
        /// <summary>
        /// 특정 이름의 자식 오브젝트 반환
        /// 자식이 많을 수록 성능에 영향이 있을 수 있으므로 사용시 주의!
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform FindDeepChild(this Transform parent, string childName)
        {
            // Transform은 IEnumerable을 상속받고 있으므로, foreach문을 사용할 수 있다.
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                    return child;
        
                Transform found = FindDeepChild(child, childName);
                if (found != null)
                    return found;
            }
            return null;
        }
    }    
}
/*
 * Scroll View(ScrollRect) > Viewport > Content의 자식으로 프리팹을 생성하고, 해당 프리팹의 컴포넌트로 붙여 사용합니다.
 */

using ReusableScrollSample.Util.UI;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

namespace ReusableScrollSample.UI.ScrollView
{
    // 스크롤뷰 아이템 프리팹에 attached
    public class StageScrollViewItem : MonoBehaviour, IScrollViewItem<StageData>
    {
        public Text titleText;
        
        private int stageIndex;

        public void SetData(int index, StageData data)
        {
            stageIndex = index;
            titleText.text = data.Title;
        }

        public void OnClickButton()
        {
            Debug.Log(stageIndex);
        }
    }
}
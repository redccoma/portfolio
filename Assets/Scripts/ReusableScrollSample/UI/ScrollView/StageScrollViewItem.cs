/*
 * Scroll View(ScrollRect) > Viewport > Content의 자식으로 프리팹을 생성하고, 해당 프리팹의 컴포넌트로 붙여 사용합니다.
 */

using System;
using ReusableScrollSample.Util.UI;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

namespace ReusableScrollSample.UI.ScrollView
{
    // 스크롤뷰 아이템 프리팹에 attached
    public class StageScrollViewItem : MonoBehaviour, IScrollViewItem<StageData>
    {
        public Text titleText;
        
        private StageData mData;
        private Action<StageData> onClickStageItem;

        public void SetData(StageData data, Action<StageData> callback)
        {
            mData = data;
            titleText.text = data.Title;
            onClickStageItem = callback;
        }

        public void OnClickButton()
        {
            onClickStageItem?.Invoke(mData);
        }
    }
}
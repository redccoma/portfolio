/*
 * Scroll View(ScrollRect) > Viewport > Content의 자식으로 프리팹을 생성하고, 해당 프리팹의 컴포넌트로 붙여 사용합니다.
 */

using System;
using ReusableScrollSample.Util.UI;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

namespace Game.GameLobby
{
    // 스크롤뷰 아이템 프리팹에 attached
    public class StageScrollViewItem : MonoBehaviour, IScrollViewItem<StageData>
    {
        public Action<StageData> OnClickStageItem;
        
        [SerializeField]
        private Text titleText;
        
        private StageData mData;

        public void SetData(StageData data, Action<StageData> callback)
        {
            mData = data;
            titleText.text = data.Title;
            OnClickStageItem = callback;
        }

        public void OnClickButton()
        {
            OnClickStageItem?.Invoke(mData);
        }
    }
}
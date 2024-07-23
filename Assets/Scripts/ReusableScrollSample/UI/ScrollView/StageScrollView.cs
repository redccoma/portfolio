/*
 * ReusableScrollView 을 사용하여 실제 스크롤뷰를 처리하기 위한 스크립트.
 * StageScrollView 컴포넌트를 ScrollRect가 있는 객체에 붙여서 사용합니다.
 */

using ReusableScrollSample.Util.UI;

namespace ReusableScrollSample.UI.ScrollView
{
    public class StageData
    {
        public readonly string Title;
        public readonly int Index;

        public StageData(string title, int index)
        {
            Title = title;
            Index = index;
        }
    }
    
    public class StageScrollView : ReusableScrollView<StageData> { }
}
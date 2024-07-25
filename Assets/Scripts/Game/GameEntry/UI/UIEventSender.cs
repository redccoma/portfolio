/*
 * UI에서 발생되는 이벤트 송신을 담당합니다.
 */

using Game.Util.EventHandler;

namespace Game.GameEntry.UI
{
    public class UIEventSender : EventHandlerBase 
    {
        public static UIEventSender Instance { get; private set; }
        
        public enum Event
        {
            StartButtonClick
        }

        private void Awake()
        {
            Instance = this;
        }

        public void OnClickStartButton()
        {
            DoEvent(Event.StartButtonClick);
        }
    }    
}
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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
            DoEvent(Event.StartButtonClick, 1);
        }
    }    
}
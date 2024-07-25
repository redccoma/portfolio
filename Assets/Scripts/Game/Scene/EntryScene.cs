using System;

using UnityEngine;

using Game.GameEntry.UI;

namespace Game.Scene
{
    using SceneManager = Game.Manager.SceneManager;
    
    public class EntryScene : MonoBehaviour
    {
        private UIEventSender eventSender;
        
        private void Start()
        {
            eventSender = UIEventSender.Instance;

            BindGameEvents();
        }

        private void BindGameEvents()
        {
            eventSender.BindEvent(UIEventSender.Event.StartButtonClick, OnClickStartButton);
        }

        private void OnClickStartButton(object[] args)
        {
            SceneManager.LoadScene(SceneManager.SceneType.Lobby);
        }
    }
}
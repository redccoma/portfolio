/*
 * Lobby 씬 전체를 담당합니다.
 */

using System.Collections.Generic;
using UnityEngine;

using Game.GameLobby;
using Game.Manager;

namespace Game.Scene
{
    public class LobbyScene : MonoBehaviour
    {
        public StageScrollView scrollView;
        
        private void Start()
        {
            List<StageData> dataList = new List<StageData>();
            for (int i = 0; i < 100; i++)
            {
                StageData data = new StageData($"Item {i}", i);
                dataList.Add(data);
            }
            
            scrollView.UpdateData(dataList, OnClickStageItem);
        }
        
        private void OnClickStageItem(StageData data)
        {
            GoStage(data.Index);
        }

        private void GoStage(int index)
        {
            switch (index)
            {
                case 0:
                    SceneManager.LoadScene(SceneManager.SceneType.Lobby, SceneManager.SceneType.Stage1);
                    break;
                case 1:
                    SceneManager.LoadScene(SceneManager.SceneType.Lobby, SceneManager.SceneType.Stage2);
                    break;
                case 2:
                    SceneManager.LoadScene(SceneManager.SceneType.Lobby, SceneManager.SceneType.Stage3);
                    break;
            }
        }
    }    
}
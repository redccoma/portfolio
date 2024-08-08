/*
 * Lobby 씬 전체를 담당합니다.
 */

using System.Collections.Generic;
using UnityEngine;

using Game.Manager;

namespace Game.GameLobby
{
    public class LobbyScene : MonoBehaviour
    {
        private int stageCount = 2;
        
        public StageScrollView scrollView;
        
        private void Start()
        {
            List<StageData> dataList = new List<StageData>();
            for (int i = 0; i < stageCount; i++)
            {
                StageData data = new StageData($"Stage {i}", i);
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
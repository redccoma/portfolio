/*
 * Stage 씬 전체를 관리합니다.
 */

using Game.Manager;
using UnityEngine;

namespace Game.GameStage3
{
    public class Stage3Scene : MonoBehaviour
    {

        public void OnExitButton()
        {
            SceneManager.LoadScene(SceneManager.SceneType.Stage3, SceneManager.SceneType.Lobby);
        }
    }    
}
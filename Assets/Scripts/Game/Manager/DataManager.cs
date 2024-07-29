/*
 * 게임 전체 데이터를 관리합니다.
 */

using UnityEngine;

namespace Game.Manager
{
    public class DataManager : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // 엔트리씬 진입. 만약 처리할 데이터가 있다면 처리하고 진입시켜도 될듯.
            SceneManager.DirectLoadScene(SceneManager.SceneType.Entry);
        }
    }
}
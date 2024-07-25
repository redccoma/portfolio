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
    }
}
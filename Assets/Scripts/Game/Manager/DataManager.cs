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
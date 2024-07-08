/*
 가장 최근 플레이한 데이터를 서버통신없이 클라이언트에 저장하려는 목적의 코드입니다.
*/

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Srtipts
{
    [System.Serializable]
    public class RecentInfo
    {
        private const int MAX_RECENT_COUNT = 5;

        // 로컬에 저장될 실 데이터.
        public Queue<int> Queue = new Queue<int>();

        public void AddGroup(int code)
        {
            if (Queue == null)
                Queue = new Queue<int>();

            if (Queue.Count >= MAX_RECENT_COUNT)
            {
                int removeCount = (MAX_RECENT_COUNT - Queue.Count) + 1; 
                for (int i = 0; i < removeCount; i++)
                    Queue.Dequeue();
            }

            if(!Queue.Contains(code))
                Queue.Enqueue(code);
        }
    }

    public static class RecentInfoUtil
    {
        public static RecentInfo GetRecentInfo()
        {
            string str = UnityEngine.PlayerPrefs.GetString("RecentInfo", string.Empty);
            RecentInfo info = null;
            try
            {
                if (!string.IsNullOrEmpty(str))
                    info = JsonConvert.DeserializeObject<RecentInfo>(str);
                
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RecentSongInfoUtil] GetRecentSongInfo() - Deserialize Error: {e.Message}");
                UnityEngine.PlayerPrefs.DeleteKey("RecentSongInfo");
                
            }
            
            if (info == null)
                info = new RecentInfo();
            
            return info;
            
        }

        public static void SetRecentSongInfo(this RecentInfo info, int groupCode)
        {
            info.AddGroup(groupCode);
            string str = JsonConvert.SerializeObject(info);
            UnityEngine.PlayerPrefs.SetString("RecentSongInfo", str);
            UnityEngine.PlayerPrefs.Save();
        }
    }
}
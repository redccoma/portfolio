/*
 가장 최근 플레이한 데이터를 서버통신없이 클라이언트에 저장하려는 목적의 코드입니다. 
 저장시 동일 요소는 배제합니다.
*/

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace RecentInfoSample
{
    [System.Serializable]
    public class RecentInfo
    {
        private const int MAX_COUNT = 5;
        
        private class InternalData
        {
            public List<int> DataList { get; set; } = new List<int>();
        }
        
        [JsonProperty]
        private InternalData data = new InternalData();

        public void AddItem(int code)
        {
            int index = data.DataList.IndexOf(code);
            if (index != -1)
                data.DataList.RemoveAt(index);
            
            data.DataList.Add(code);
            
            TrimList();
        }

        public IReadOnlyList<int> GetList()
        {
            TrimList();
            return data.DataList.AsReadOnly();
        }
        
        private void TrimList()
        {
            if (data.DataList.Count > MAX_COUNT)
            {
                data.DataList.RemoveRange(0, data.DataList.Count - MAX_COUNT);
            }
        }
    }

    public static class RecentInfoUtil
    {
        private const string KEY = "RecentInfo";
        
        /// <summary>
        /// 최근 정보를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public static RecentInfo GetRecentInfo()
        {
            string str = PlayerPrefs.GetString(KEY, string.Empty);
            RecentInfo info = null;
            try
            {
                if (!string.IsNullOrEmpty(str))
                    info = JsonConvert.DeserializeObject<RecentInfo>(str);
                
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[RecentInfoUtil] GetRecentInfo() - Deserialize Error: {e.Message}\nStackTrace: {e.StackTrace}");
                UnityEngine.PlayerPrefs.DeleteKey(KEY);
            }
            
            if (info == null)
                info = new RecentInfo();
            
            return info;
            
        }

        /// <summary>
        /// 최근 정보를 저장합니다.
        /// </summary>
        /// <param name="info">GetRecentInfo로 획득한 데이터</param>
        /// <param name="code">저장한 코드</param>
        public static void SetRecentInfo(this RecentInfo info, int code)
        {
            if(info == null)
                info = new RecentInfo();
            
            info.AddItem(code);
            string str = JsonConvert.SerializeObject(info);
            PlayerPrefs.SetString(KEY, str);
        }

        /// <summary>
        /// 저장되어 있는 모든 사항을 디스크에 즉시 기록합니다.
        /// 성능 이슈가 있을 수 있으므로 필요할때 호출.
        /// </summary>
        public static void SaveChange()
        {
            PlayerPrefs.Save();
        }
    }
}
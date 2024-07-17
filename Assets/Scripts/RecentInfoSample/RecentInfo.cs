/*
 가장 최근 플레이한 데이터를 서버통신없이 PlayerPrefs를 사용하여 로컬에 저장하려는 목적의 코드입니다. 
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

    public static class RecentInfoHelper
    {
        private const string KEY = "RecentInfo";

        public static RecentInfo GetRecentInfo()
        {
            return LocalDataUtil<RecentInfo>.GetData(KEY);
        }

        public static void SetRecentInfo(RecentInfo info, int code)
        {
            if (info == null)
                info = new RecentInfo();
            
            info.AddItem(code);
            LocalDataUtil<RecentInfo>.SetData(KEY, info);
        }
    }
    
    // RecentInfo와 비슷한 데이터들을 읽고 쓰기 편하도록 하는 범용 클래스입니다.
    public static class LocalDataUtil<T> where T : class, new()
    {
        /// <summary>
        /// 로컬 데이터를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public static T GetData(string key)
        {
            string str = PlayerPrefs.GetString(key, string.Empty);
            T data = null;
            try
            {
                if (!string.IsNullOrEmpty(str))
                    data = JsonConvert.DeserializeObject<T>(str);
                
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[LocalDataUtil] GetData() - Deserialize Error: {e.Message}\nStackTrace: {e.StackTrace}");
                PlayerPrefs.DeleteKey(key);
            }
            
            return data ?? new T();
        }

        /// <summary>
        /// 최근 정보를 저장합니다.
        /// </summary>
        /// <param name="key">PlayerPrefs 키</param>
        /// <param name="data">저장할 데이터객체</param>
        public static void SetData(string key, T data)
        {
            if(data == null)
                data = new T();
            
            string str = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, str);
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
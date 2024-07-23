using System.Collections.Generic;
using ReusableScrollSample.UI.ScrollView;
using UnityEngine;

namespace ReusableScrollSample.Manager
{   
    public class DataManager : MonoBehaviour
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
            
            scrollView.UpdateData(dataList);
        }
    }

}
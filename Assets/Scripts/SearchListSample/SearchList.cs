/*
 List<T>에 저장된 데이터에서 특정 키워드에 해당하는 데이터를 검색하여 반환합니다.
*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SearchListSample
{
    public class SearchList : MonoBehaviour
    {
        public class Data
        {
            public int code;
            
            public string Name;
            public string Alias;
            public string Gender;
        }
        private static List<Data> dataList = new List<Data>();
        
        private static Dictionary<string, HashSet<Data>> nameList = new Dictionary<string, HashSet<Data>>();
        private static Dictionary<string, HashSet<Data>> aliasList = new Dictionary<string, HashSet<Data>>();
        private static Dictionary<string, HashSet<Data>> genderList = new Dictionary<string, HashSet<Data>>();

        private string[] firstNames = { "John", "Jane", "Mike", "Emily", "Alex", "Sarah", "Chris", "Emma", "David", "Olivia" };
        private string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
        private string[] aliasDataSet = {
            "Shadow", "Blaze", "Frost", "Storm", "Nova", "Echo", "Raven", "Phoenix", "Luna", "Orion",
            "Titan", "Viper", "Specter", "Zephyr", "Crimson", "Nebula", "Quantum", "Apex", "Enigma", "Omega",
            "Cypher", "Wraith", "Frostbite", "Nimbus", "Solstice", "Vortex", "Onyx", "Aether", "Zenith", "Phantom",
            "Maverick", "Neon", "Avalanche", "Blitz", "Cosmos", "Dauntless", "Elysium", "Firefly", "Gravity", "Havoc",
            "Inferno", "Jade", "Knightfall", "Labyrinth", "Mirage", "Nexus", "Oracle", "Pulsar", "Quasar", "Ronin"
        };
        
        // 샘플 코드 입력
        public void Start()
        {
            for (int i = 0; i < 100; i++)
            {
                Data data = new Data
                {
                    code = i,
                    Name = GetName(),
                    Alias = GetAlias(),
                    Gender = GetGender()
                };
                
                dataList.Add(data);
                
                // 검색 인덱스에 추가
                AddToSearchList(nameList, data.Name.ToLower(), data);
                AddToSearchList(aliasList, data.Alias.ToLower(), data);
                AddToSearchList(genderList, data.Gender.ToLower(), data);
            }
        }
        
        private string GetName()
        {
            string firstName = firstNames[Random.Range(0, firstNames.Length)];
            string lastName = lastNames[Random.Range(0, lastNames.Length)];
            return $"{firstName} {lastName}";
        }
        
        private string GetAlias()
        {
            return aliasDataSet[Random.Range(0, aliasDataSet.Length)];
        }

        private string GetGender()
        {
            float randomValue = Random.value;
            if (randomValue < 0.51f)
                return "man";
            
            if (randomValue < 0.99f)
                return "women";
            
            return "etc"; // 매우 낮은 확률로 '기타' 옵션 추가
        }

        // 검색어에 해당하는 데이터가 있다면 해당하는 코드를 리스트로 반환
        public static List<int> Search(string text)
        {
            text = text.ToLower();
            var results = new HashSet<Data>();

            SearchInIndex(nameList, text, results);
            SearchInIndex(aliasList, text, results);
            SearchInIndex(genderList, text, results);

            return results.Select(data => data.code).ToList();
        }
            
        private static void AddToSearchList(Dictionary<string, HashSet<Data>> index, string key, Data data)
        {
            if (!index.ContainsKey(key))
                index[key] = new HashSet<Data>();
            index[key].Add(data);
        }
            
        private static void SearchInIndex(Dictionary<string, HashSet<Data>> dict, string text, HashSet<Data> results)
        {
            foreach (var key in dict.Keys)
            {
                if (key.Contains(text))
                {
                    results.UnionWith(dict[key]);
                }
            }
        }
    }
}
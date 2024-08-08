/*
 * Stage 씬 전체를 관리합니다.
 */

using System;
using System.Collections.Generic;
using Game.GameLoading;
using UnityEngine;
using UnityEngine.Tilemaps;

using Game.Manager;
using Random = UnityEngine.Random;
using Text = TMPro.TextMeshProUGUI;

namespace Game.GameStage2
{
    public class Stage2Scene : MonoBehaviour
    {
        private static Stage2Scene instance;
        
        public GameObject monsterPrefab;    // 몬스터 프리팹
     
        public float spawnInterval = 0.5f;  // 몹 생성 주기
        public Tilemap groundTilemap;     // 지면 타일맵
        public Tilemap wallTilemap;       // 벽 타일맵
        public Text scoreText;          // 점수 표시
        public MultiImageProgressbar healthBar; // 체력바
        public Player player;        // 플레이어
        
        private int currentCount = 0;   // 현재까지 스폰된 몹 갯수
        private int maxCount = 1000;    // 최대 스폰갯수 
        private float timeSinceLastSpawn = 0f;  // 스폰시간 조절 변수
        private List<Monster> activeMonsters = new List<Monster>(); // 활성화된 몹 리스트
        private bool isStop;    // 게임중지상태
        private bool init;    // 초기화 여부
        private int deadMonsterCount = 0;   // 죽은 몹 갯수
        private readonly int maxDeadMonsterCount = 100;  // 목표치(죽은 몹 갯수)
        
        public static bool IsStop => !instance || instance.isStop;

        private void Awake()
        {
            instance = this;
            init = false;
            
            SetScore();
            
            // 로딩 씬이 완전히 끝난후 stage 시작.
            LoadingScene.OnLoadingComplete -= Init;
            LoadingScene.OnLoadingComplete += Init;
        }

        private void Init()
        {
            player.Init(PlayerDead);
            player.OnHealthChanged += OnPlayerHealthChanged;

            init = true;
            
            LoadingScene.OnLoadingComplete -= Init;
        }

        private void OnPlayerHealthChanged(int maxHealth, int currentHealth)
        {
            healthBar.SetProgress((float)currentHealth / maxHealth, currentHealth);
        }

        private void Update()
        {
            if (IsStop || !init)
                return;
            
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnInterval)
            {
                if (currentCount < maxCount)
                {
                    SpawnPrefab();
                    currentCount++;    
                }
                
                timeSinceLastSpawn = 0f;
            }
        }

        private void SpawnPrefab()
        {
            Vector3Int randomCell = GetRandomCellPosition();
            Vector3 worldPosition = groundTilemap.GetCellCenterWorld(randomCell);

            if (IsValidSpawnPosition(randomCell))
            {
                GameObject monsterObject = Instantiate(monsterPrefab, worldPosition, Quaternion.identity);
                Monster monster = monsterObject.GetComponent<Monster>();
                monster.Setup(player.transform, MonsterHitCallback);
                activeMonsters.Add(monster);
            }
        }

        // 전체 맵안에서 랜덤 위치 반환
        private Vector3Int GetRandomCellPosition()
        {
            BoundsInt bounds = groundTilemap.cellBounds;
            int x = Random.Range(bounds.xMin, bounds.xMax);
            int y = Random.Range(bounds.yMin, bounds.yMax);
            return new Vector3Int(x, y, 0);
        }

        private bool IsValidSpawnPosition(Vector3Int cellPosition)
        {
            // 해당 위치에 지면 타일이 있고, 벽 타일이 없는지 확인
            return groundTilemap.HasTile(cellPosition) && !wallTilemap.HasTile(cellPosition);
        }

        private void MonsterHitCallback(int remainMonsterHealth)
        {
            if(remainMonsterHealth <= 0)
            {
                deadMonsterCount++;
                SetScore();
            }
            
            if(deadMonsterCount >= maxDeadMonsterCount && !isStop)
            {
                isStop = true;
                
                Monster[] monsters = GetActiveMonsters();
                foreach (var m in monsters)
                    m.ClearObject();
                
                SceneManager.LoadScene(SceneManager.SceneType.Stage2, SceneManager.SceneType.Lobby);
            }
        }
        
        public static Monster[] GetActiveMonsters()
        {
            if(instance == null)
                return Array.Empty<Monster>();
            
            instance.activeMonsters.RemoveAll(m => m == null);
            
            return instance.activeMonsters.ToArray();
        }

        private void PlayerDead()
        {
            instance.isStop = true;
            
            SceneManager.LoadScene(SceneManager.SceneType.Stage2, SceneManager.SceneType.Lobby);
        }

        private void SetScore()
        {
            scoreText.text = $"Count: {deadMonsterCount} / Target: {maxDeadMonsterCount}";
        }
    }    
}
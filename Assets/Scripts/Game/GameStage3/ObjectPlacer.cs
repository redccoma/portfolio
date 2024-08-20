using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Game.GameStage3
{
    public class ObjectPlacer : MonoBehaviour
    {
        public Camera mainCamera;
        public List<ExtendedTileSet> tileSets;
        public UIButtonPressTracker treeButton;
        public UIButtonPressTracker aniButton;
        
        private TilemapGridSystem gridSystem;
        private List<Vector3Int> extendGridPosition = new List<Vector3Int>();

        private ExtendedTileSet currentTileSet;
        private bool isPressButton = false;
        private Vector3Int? nullablePosition;   // 최적화를 위해 값을 null 형태로 사용하여 비교/처리.

        private void Awake()
        {
            gridSystem = GetComponent<TilemapGridSystem>();
            
            treeButton.onButtonPressed.AddListener((type, isPress) => PressButton(type, isPress));
            aniButton.onButtonPressed.AddListener((type, isPress) => PressButton(type, isPress));
        }

        private void PressButton(TileType type, bool isPress)
        {
            currentTileSet = GetTileSet(type);
            nullablePosition = null;
            
            if (!isPress)
            {
                // 버튼을 뗀 순간이므로
                gridSystem.ClearIndicatorTileMap();
                gridSystem.CreateItem(currentTileSet);
            }
            else
            {
                // 버튼을 누르는 순간.
            }
            
            isPressButton = isPress;
        }
        
        // 타일 타입에 따른 tileset 확인.
        private ExtendedTileSet GetTileSet(TileType type)
        {
            foreach (var tileSet in tileSets)
            {
                if (tileSet.TileSetType == type)
                {
                    return tileSet;
                }
            }

            Debug.LogWarning($"타일SET 지정이 안되어 있습니다. {type}");
            return null;
        }
        
        /// <summary>
        /// 마우스 포인터 기반 셀위치값에서 tileset 영역만큼 선택 영역을 확장
        /// 필드변수 extendGridPosition에 저장됨.
        /// </summary>
        /// <param name="tileSet"></param>
        /// <param name="gridPosition"></param>
        private void CalcGridPositions(ExtendedTileSet tileSet, Vector3Int gridPosition)
        {
            extendGridPosition.Clear();
            
            for (int y = 0; y < tileSet.Height; y++)
            {
                for (int x = 0; x < tileSet.Width; x++)
                {
                    Vector3Int tilePosition = new Vector3Int(gridPosition.x - x, gridPosition.y + y, 0);    // 오른쪽 하단 기준.
                    extendGridPosition.Add(tilePosition);
                }
            }
            
            extendGridPosition = extendGridPosition
                .OrderBy(v => v.y)  // 먼저 Y 좌표로 오름차순 정렬(아래에서 위로)
                .ThenBy(v => v.x)   // 그 다음 X 좌표로 오름차순 정렬(왼쪽에서 오른쪽으로)
                .ToList();
        }

        // indicator 타일맵에 지을수 있는지 여부 보여주기.
        void Update()
        {
            if (isPressButton)
            {
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPosition = gridSystem.GetGridPosition(mouseWorldPos);
                
                // 현재 위치가 이전에 계산했던 위치와 동일하다면 계산하지 않는다.
                if(nullablePosition.HasValue && nullablePosition.Value == gridPosition)
                    return;

                nullablePosition = gridPosition;
                CalcGridPositions(currentTileSet, gridPosition);
                
                // Indicator 타일맵에 지을수 있는지 여부를 보여주고 up 상태
                gridSystem.UpdatePlacementIndicators(currentTileSet, extendGridPosition);
            }
        }
    }
}
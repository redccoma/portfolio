using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Game.GameStage3
{
    public class TilemapGridSystem : MonoBehaviour
    {
        public Tilemap groundTilemap; // 기본 지형 타일맵 (건물/오브젝트 생성 타일)
        public Tilemap indicatorTilemap; // 생성 가능표시 타일맵

        private List<bool> enableXposition = new List<bool>();  // tileSetY가 0인 타일의 활성화 여부를 저장하는 리스트
        private List<Vector3Int> selectedArea;
        private bool isCanMake;

        void Start()
        {
            if (groundTilemap == null || indicatorTilemap == null)
                return;
        }

        /// <summary>
        /// indicator 타일맵 업데이트
        /// </summary>
        /// <param name="tileSet">생성하려는 타일Set 데이터</param>
        /// <param name="gridArea">표시 여부를 확인할 셀포지션 영역</param>
        public void UpdatePlacementIndicators(ExtendedTileSet tileSet, List<Vector3Int> gridArea)
        {
            ClearIndicatorTileMap();
            
            enableXposition.Clear();

            selectedArea = gridArea;
            isCanMake = true;
            
            // 그려야할 타일set을 먼저 indicator 타일에 set.
            int index = 0;
            for (int y = 0; y < tileSet.Height; y++)
            {
                for (int x = 0; x < tileSet.Width; x++)
                {
                    TileBase originalTile = tileSet.GetTile(x, y);
                    Vector3Int tilePosition = gridArea[index++];
            
                    if (originalTile != null)
                    {
                        bool isEnable = IsPlaceablePosition(tilePosition);
                        if (y == 0)
                            enableXposition.Add(isEnable);
                        else
                            isEnable = enableXposition[x];
                        
                        Color tileColor = isEnable ? Color.white : Color.red;
                        if (originalTile is AnimatedTile animTile)
                        {
                            // animatedTile은 자체 컬러를 사용하지 않으므로
                            // 새로운 타일을 생성할 필요없이 타일맵 컬러를 변경하면 됨.
                            indicatorTilemap.SetTile(tilePosition, animTile);
                            indicatorTilemap.SetColor(tilePosition, tileColor);
                        }
                        else if (originalTile is Tile tile)
                        {
                            // Tile은 자체 컬러를 사용하므로, 타일맵 컬러가 아닌 타일 속성에서 컬러를 변경하고
                            // 변경된 타일을 SetTile.
                            tile.color = tileColor;
                            indicatorTilemap.SetTile(tilePosition, tile);
                        }
                        
                        if (isCanMake && !isEnable)
                            isCanMake = false;
                    }
                }
            }
        }

        // selectedArea에 저장된 영역에 ground타일맵에 타일 그리기.
        public void CreateItem(ExtendedTileSet tileSet)
        {
            if (isCanMake)
            {
                int index = 0;
                for (int y = 0; y < tileSet.Height; y++)
                {
                    for (int x = 0; x < tileSet.Width; x++)
                    {
                        TileBase originalTile = tileSet.GetTile(x, y);
                        Vector3Int tilePosition = selectedArea[index++];
                        
                        if (originalTile != null)
                        {
                            groundTilemap.SetTile(tilePosition, originalTile);
                        }
                    }
                }
            }
        }
        
        // indicator 타일맵 전체 초기화.
        public void ClearIndicatorTileMap()
        {
            indicatorTilemap.ClearAllTiles();
        }

        // 특정 그리드 위치(월드 포지션)에 오브젝트를 배치할 수 있는지 확인
        private bool IsPlaceablePosition(Vector3Int gridPosition)
        {
            // 지으려는 공간 바로 아래 지형타일이 있어야 한다.
            
            bool isCurrentTile = groundTilemap.HasTile(gridPosition);
            bool isDownTile = groundTilemap.HasTile(gridPosition + Vector3Int.down);

            return isDownTile && !isCurrentTile;
        }

        // 지으려는 위치에 충돌 객체 있는지 검사 (타일이 아닌 다른 객체가 있을 경우 사용하면 될듯)
        private bool IsOccupied(Vector3Int gridPosition)
        {
            Collider2D collider = Physics2D.OverlapPoint(GetWorldPosition(gridPosition));
            return collider != null;
        }

        // 그리드 위치를 월드 좌표로 변환
        private Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            return groundTilemap.GetCellCenterWorld(gridPosition);
        }

        // 월드 포지션을 그리드 위치로 변경
        public Vector3Int GetGridPosition(Vector3 worldPosition)
        {
            return groundTilemap.WorldToCell(worldPosition);
        }
    }
}
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.GameStage3
{
    public class TilemapGridSystem : MonoBehaviour
    {
        public Tilemap groundTilemap; // 기본 지형 타일맵
        public Tilemap indicatorTilemap; // 배치 가능/불가능 영역을 표시할 타일맵
        public TileBase placementPossibleTile; // 배치 가능 영역을 표시할 타일
        public TileBase placementImpossibleTile; // 배치 불가능 영역을 표시할 타일

        private BoundsInt bounds;

        void Start()
        {
            if (groundTilemap == null || indicatorTilemap == null)
            {
                Debug.LogError("Tilemaps not assigned!");
                return;
            }

            bounds = groundTilemap.cellBounds;
            UpdatePlacementIndicators();
        }

        void UpdatePlacementIndicators()
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    if (groundTilemap.HasTile(tilePosition)) // 지형 타일이 있는 경우에만 표시
                    {
                        if (IsPlaceablePosition(tilePosition))
                        {
                            indicatorTilemap.SetTile(tilePosition, placementPossibleTile);
                        }
                        else
                        {
                            indicatorTilemap.SetTile(tilePosition, placementImpossibleTile);
                        }
                    }
                }
            }
        }

        public bool IsPlaceablePosition(Vector3Int gridPosition)
        {
            // 여기서 배치 가능한 위치의 조건을 정의합니다.
            // 예: 지형 타일맵에 타일이 있고, 다른 오브젝트가 없는 경우
            return groundTilemap.HasTile(gridPosition) && !IsOccupied(gridPosition);
        }

        private bool IsOccupied(Vector3Int gridPosition)
        {
            // 이 메서드를 구현하여 해당 그리드 위치에 오브젝트가 있는지 확인
            // 예: 콜라이더 체크 또는 별도의 데이터 구조를 사용하여 관리
            Collider2D collider = Physics2D.OverlapPoint(GetWorldPosition(gridPosition));
            return collider != null;
        }

        public Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            return groundTilemap.GetCellCenterWorld(gridPosition);
        }

        public Vector3Int GetGridPosition(Vector3 worldPosition)
        {
            return groundTilemap.WorldToCell(worldPosition);
        }

        public bool CanPlaceObjectAt(Vector3Int gridPosition)
        {
            return IsPlaceablePosition(gridPosition);
        }

        public void PlaceObject(GameObject obj, Vector3Int gridPosition)
        {
            if (CanPlaceObjectAt(gridPosition))
            {
                Vector3 worldPos = GetWorldPosition(gridPosition);
                obj.transform.position = worldPos;
                indicatorTilemap.SetTile(gridPosition, placementImpossibleTile); // 오브젝트가 배치되면 불가능 영역으로 표시
                UpdateAdjacentTiles(gridPosition); // 주변 타일 업데이트
            }
        }

        public void RemoveObject(Vector3Int gridPosition)
        {
            if (groundTilemap.HasTile(gridPosition))
            {
                indicatorTilemap.SetTile(gridPosition, placementPossibleTile);
                UpdateAdjacentTiles(gridPosition); // 주변 타일 업데이트
            }
        }

        private void UpdateAdjacentTiles(Vector3Int position)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue; // 중앙 타일 제외
                    Vector3Int adjacentPosition = position + new Vector3Int(x, y, 0);
                    if (groundTilemap.HasTile(adjacentPosition))
                    {
                        if (IsPlaceablePosition(adjacentPosition))
                        {
                            indicatorTilemap.SetTile(adjacentPosition, placementPossibleTile);
                        }
                        else
                        {
                            indicatorTilemap.SetTile(adjacentPosition, placementImpossibleTile);
                        }
                    }
                }
            }
        }
    }
}
/*
 * 2D MAP을 타일맵을 사용해 랜덤하게 생성
 */

using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Game.GameStage1
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int mapSize;
        [SerializeField]
        private float minimumDivideRate; //공간이 나눠지는 최소 비율
        [SerializeField]
        private float maximumDivideRate; //공간이 나눠지는 최대 비율
        [SerializeField]
        private int maxIteration; //트리의 높이, 높을 수록 방을 더 자세히 나누게 됨

        [SerializeField]
        private Tilemap itemTilemap;
        [SerializeField]
        private Tilemap groundTilemap;
        [SerializeField]
        private Tile roomTile; //방을 구성하는 타일

        [SerializeField]
        private Tile itemTile;  // 횃불증가 아이템 타일 
        [SerializeField] 
        private Tile wallTile;
        [SerializeField]
        private Tile escapeTile;
        
        [SerializeField] 
        private Tile outTile; //방 외부의 타일

        [SerializeField]
        private Transform player;

        private const float ITEM_SPAWN_CHANCE = 0.1f;
        private const int BACKGROUND_PADDING = 10;
        
        private List<Room> leafRooms = new List<Room>();

        private void Start()
        {
            GenerateMap();
            PlaceEscapeTileAndPlayer();
        }
        
        private void PlaceEscapeTileAndPlayer()
        {
            if (leafRooms.Count < 2)
            {
                Debug.LogError("Not enough rooms to place escape tile and player");
                return;
            }

            Room[] furthestRooms = FindFurthestRooms();
            PlaceEscapeTile(furthestRooms[0]);
            PlacePlayer(furthestRooms[1]);
        }
        
        private Room[] FindFurthestRooms()
        {
            float maxDistance = 0;
            Room[] furthestRooms = new Room[2];

            for (int i = 0; i < leafRooms.Count; i++)
            {
                for (int j = i + 1; j < leafRooms.Count; j++)
                {
                    float distance = Vector2.Distance(leafRooms[i].center, leafRooms[j].center);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        furthestRooms[0] = leafRooms[i];
                        furthestRooms[1] = leafRooms[j];
                    }
                }
            }

            return furthestRooms;
        }
        
        private void PlaceEscapeTile(Room room)
        {
            Vector3Int tilePosition = GetRandomPositionInRoom(room);
            SetItemTile(tilePosition, escapeTile);
        }

        private void PlacePlayer(Room room)
        {
            Vector3Int tilePosition = GetRandomPositionInRoom(room);
            Vector3 worldPosition = groundTilemap.GetCellCenterWorld(GetAdjustedPosition(tilePosition.x, tilePosition.y));
            player.position = worldPosition;
        }
        
        private Vector3Int GetRandomPositionInRoom(Room room)
        {
            int x = Random.Range(room.rect.x, room.rect.x + room.rect.width);
            int y = Random.Range(room.rect.y, room.rect.y + room.rect.height);
            return new Vector3Int(x, y, 0);
        }

        private void GenerateMap()
        {
            FillBackground();
            
            Room rootRoom = new Room(new RectInt(0, 0, mapSize.x, mapSize.y));
            
            DivideRoom(rootRoom, 0);
            GenerateRooms(rootRoom, 0);
            GenerateCorridors(rootRoom, 0);
            FillWalls();
        }

        private void DivideRoom(Room room, int iteration)
        {
            if (iteration == maxIteration) return;

            int maxLength = Mathf.Max(room.rect.width, room.rect.height);
            int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDivideRate, maxLength * maximumDivideRate));
            
            room.PartitionRoom(split);

            DivideRoom(room.partitionA, iteration + 1);
            DivideRoom(room.partitionB, iteration + 1);
        }

        private void GenerateRooms(Room room, int iteration)
        {
            if (iteration == maxIteration)
            {
                room.rect = GenerateRoom(room.rect);
                leafRooms.Add(room);
                return;
            }
            
            GenerateRooms(room.partitionA, iteration + 1);
            GenerateRooms(room.partitionB, iteration + 1);
        }

        private RectInt GenerateRoom(RectInt rect)
        {
            int width = Random.Range(rect.width / 2, rect.width - 1);
            int height = Random.Range(rect.height / 2, rect.height - 1);
            int x = rect.x + Random.Range(1, rect.width - width);
            int y = rect.y + Random.Range(1, rect.height - height);
            
            RectInt roomRect = new RectInt(x, y, width, height);
            FillRoom(roomRect);
            return roomRect;
        }

        private void GenerateCorridors(Room room, int iteration)
        {
            if (iteration == maxIteration) return;
            
            Vector2Int leftCenter = room.partitionA.center2Int;
            Vector2Int rightCenter = room.partitionB.center2Int;

            DrawHorizontalCorridor(leftCenter, rightCenter);
            DrawVerticalCorridor(leftCenter, rightCenter);

            GenerateCorridors(room.partitionA, iteration + 1);
            GenerateCorridors(room.partitionB, iteration + 1);
        }

        private void DrawHorizontalCorridor(Vector2Int start, Vector2Int end)
        {
            for (int i = Mathf.Min(start.x, end.x); i <= Mathf.Max(start.x, end.x); i++)
            {
                SetGroundTile(new Vector3Int(i, start.y, 0), roomTile);
            }
        }

        private void DrawVerticalCorridor(Vector2Int start, Vector2Int end)
        {
            for (int j = Mathf.Min(start.y, end.y); j <= Mathf.Max(start.y, end.y); j++)
            {
                SetGroundTile(new Vector3Int(end.x, j, 0), roomTile);
            }
        }

        private void FillBackground()
        {
            for (int i = -BACKGROUND_PADDING; i < mapSize.x + BACKGROUND_PADDING; i++)
            {
                for (int j = -BACKGROUND_PADDING; j < mapSize.y + BACKGROUND_PADDING; j++)
                {
                    SetGroundTile(new Vector3Int(i, j, 0), outTile);
                }
            }
        }
        
        private void FillWalls()
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    if (IsOutTile(i, j) && HasAdjacentRoomTile(i, j))
                    {
                        SetGroundTile(new Vector3Int(i, j, 0), wallTile);
                    }
                }
            }
        }

        private bool IsOutTile(int x, int y)
        {
            return groundTilemap.GetTile(GetAdjustedPosition(x, y)) == outTile;
        }

        private bool HasAdjacentRoomTile(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    if (groundTilemap.GetTile(GetAdjustedPosition(x + dx, y + dy)) == roomTile)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void FillRoom(RectInt rect)
        {
            bool isItemCreated = false;
            
            for (int i = rect.x; i < rect.x + rect.width; i++)
            {
                for (int j = rect.y; j < rect.y + rect.height; j++)
                {
                    SetGroundTile(new Vector3Int(i, j, 0), roomTile);

                    if (!isItemCreated && Random.value <= ITEM_SPAWN_CHANCE)
                    {
                        SetItemTile(new Vector3Int(i, j, 0), itemTile);
                        isItemCreated = true;
                    }
                }
            }
        }

        private void SetGroundTile(Vector3Int position, Tile tile)
        {
            groundTilemap.SetTile(GetAdjustedPosition(position.x, position.y), tile);
        }

        private void SetItemTile(Vector3Int position, Tile tile)
        {
            itemTilemap.SetTile(GetAdjustedPosition(position.x, position.y), tile);
        }

        private Vector3Int GetAdjustedPosition(int x, int y)
        {
            return new Vector3Int(x - mapSize.x / 2, y - mapSize.y / 2, 0);
        }
    }
}
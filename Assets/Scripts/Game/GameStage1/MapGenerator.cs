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
        [Header("Map Generation Parameters")]
        [SerializeField]
        private Vector2Int mapSize;
        [SerializeField]
        private float minRate;
        [SerializeField]
        private float maxRate;
        [SerializeField]
        private int maxIteration;

        [Header("Tilemaps")]
        [SerializeField]
        private Tilemap itemTilemap;
        [SerializeField]
        private Tilemap groundTilemap;
        
        [Header("Tiles")]
        [SerializeField]
        private Tile roomTile;
        [SerializeField]
        private Tile itemTile; 
        [SerializeField] 
        private Tile wallTile;
        [SerializeField]
        private Tile escapeTile;
        [SerializeField] 
        private Tile outTile;
        
        [Header("Player")]
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
        
        /// <summary>
        /// 탈출 타일과 플레이어를 가장 멀리 떨어진 방에 배치
        /// </summary>
        private void PlaceEscapeTileAndPlayer()
        {
            if (leafRooms.Count < 2)
            {
                Debug.LogError("방 갯수 부족.");
                return;
            }

            Room[] furthestRooms = FindFurthestRooms();
            PlaceEscapeTile(furthestRooms[0]);
            PlacePlayer(furthestRooms[1]);
        }
        
        /// <summary>
        /// 가장 멀리 떨어진 두 방을 찾는 메서드
        /// </summary>
        /// <returns></returns>
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
        
        /// <summary>
        /// 지정된 방에 탈출 타일 배치
        /// </summary>
        /// <param name="room"></param>
        private void PlaceEscapeTile(Room room)
        {
            Vector3Int tilePosition = GetRandomPositionInRoom(room);
            SetItemTile(tilePosition, escapeTile);
        }

        /// <summary>
        /// 지정된 방에 플레이어 배치
        /// </summary>
        /// <param name="room"></param>
        private void PlacePlayer(Room room)
        {
            Vector3Int tilePosition = GetRandomPositionInRoom(room);
            Vector3 worldPosition = groundTilemap.GetCellCenterWorld(GetAdjustedPosition(tilePosition.x, tilePosition.y));
            player.position = worldPosition;
        }
        
        /// <summary>
        /// 방 내의 랜덤한 위치 반환
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private Vector3Int GetRandomPositionInRoom(Room room)
        {
            int x = Random.Range(room.rect.x, room.rect.x + room.rect.width);
            int y = Random.Range(room.rect.y, room.rect.y + room.rect.height);
            return new Vector3Int(x, y, 0);
        }

        /// <summary>
        /// 맵 생성의 전체 프로세스를 관리
        /// </summary>
        private void GenerateMap()
        {
            FillBackground();
            
            // 전체 크기를 가진 방생성
            Room rootRoom = new Room(new RectInt(0, 0, mapSize.x, mapSize.y));
            
            DivideRoom(rootRoom, 0);
            GenerateRooms(rootRoom, 0);
            GenerateCorridors(rootRoom, 0);
            FillWalls();
        }

        /// <summary>
        /// 재귀적으로 방을 나누는 함수
        /// </summary>
        /// <param name="room"></param>
        /// <param name="iteration"></param>
        private void DivideRoom(Room room, int iteration)
        {
            if (iteration == maxIteration)
                return;

            int maxLength = Mathf.Max(room.rect.width, room.rect.height);
            int split = Mathf.RoundToInt(Random.Range(maxLength * minRate, maxLength * maxRate));
            
            room.PartitionRoom(split);

            DivideRoom(room.partitionA, iteration + 1);
            DivideRoom(room.partitionB, iteration + 1);
        }

        /// <summary>
        /// 방을 생성하고 leafRooms에 추가
        /// </summary>
        /// <param name="room"></param>
        /// <param name="iteration"></param>
        private void GenerateRooms(Room room, int iteration)
        {
            if (iteration == maxIteration)
            {
                room.rect = GenerateRoom(room.rect);
                leafRooms.Add(room);
                return;
            }
            
            // 방 나누기!
            GenerateRooms(room.partitionA, iteration + 1);
            GenerateRooms(room.partitionB, iteration + 1);
        }

        /// <summary>
        /// 매개변수 좌표와 크기를 기반으로 방생성
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 방들을 연결하는 복도를 생성
        /// </summary>
        /// <param name="room"></param>
        /// <param name="iteration"></param>
        private void GenerateCorridors(Room room, int iteration)
        {
            if (iteration == maxIteration)
                return;
            
            Vector2Int leftCenter = room.partitionA.center2Int;
            Vector2Int rightCenter = room.partitionB.center2Int;

            DrawHorizontalCorridor(leftCenter, rightCenter);
            DrawVerticalCorridor(leftCenter, rightCenter);

            GenerateCorridors(room.partitionA, iteration + 1);
            GenerateCorridors(room.partitionB, iteration + 1);
        }

        /// <summary>
        /// 가로 복도 그리기
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void DrawHorizontalCorridor(Vector2Int start, Vector2Int end)
        {
            int min = Mathf.Min(start.x, end.x);
            int max = Mathf.Max(start.x, end.x);
            
            for (int i = min; i <= max; i++)
            {
                SetGroundTile(new Vector3Int(i, start.y, 0), roomTile);
            }
        }

        /// <summary>
        /// 세로 복도 그리기
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void DrawVerticalCorridor(Vector2Int start, Vector2Int end)
        {
            int min = Mathf.Min(start.y, end.y);
            int max = Mathf.Max(start.y, end.y);
            
            for (int i = min; i <= max; i++)
            {
                SetGroundTile(new Vector3Int(end.x, i, 0), roomTile);
            }
        }

        /// <summary>
        /// outtile로 채워진 배경을 생성
        /// </summary>
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
        
        /// <summary>
        /// 벽 타일 채우기
        /// </summary>
        private void FillWalls()
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    bool isOutTile = groundTilemap.GetTile(GetAdjustedPosition(i, j)) == outTile;
                    
                    if (isOutTile && HasAdjacentRoomTile(i, j))
                    {
                        SetGroundTile(new Vector3Int(i, j, 0), wallTile);
                    }
                }
            }
        }

        /// <summary>
        /// 주변에 roomTile이 있는지 확인
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 지정된 영역에 roolTile 채우기.
        /// </summary>
        /// <param name="rect"></param>
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

        /// <summary>
        /// 맵 전체 tilemap에 타일을 배치하는 함수
        /// </summary>
        /// <param name="position">생성위치</param>
        /// <param name="tile">생성타일</param>
        private void SetGroundTile(Vector3Int position, Tile tile)
        {
            groundTilemap.SetTile(GetAdjustedPosition(position.x, position.y), tile);
        }

        /// <summary>
        /// 아이템 타일맵에 타일을 배치하는 함수
        /// </summary>
        /// <param name="position"></param>
        /// <param name="tile"></param>
        private void SetItemTile(Vector3Int position, Tile tile)
        {
            itemTilemap.SetTile(GetAdjustedPosition(position.x, position.y), tile);
        }

        /// <summary>
        /// 맵 중앙을 기준으로 조정된 위치를 반환하는 메서드
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Vector3Int GetAdjustedPosition(int x, int y)
        {
            return new Vector3Int(x - mapSize.x / 2, y - mapSize.y / 2, 0);
        }
    }
}
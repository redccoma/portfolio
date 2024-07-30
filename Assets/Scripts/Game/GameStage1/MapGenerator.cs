using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator :MonoBehaviour
{
    [SerializeField]
    private Vector2Int mapSize;
    
    [SerializeField]
    private GameObject map;
    
    private void Start()
    {  
        DrawMap(0, 0);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void DrawMap(int x, int y)
    {
        //기본적으로 mapSize/2라는 값을 계속해서 빼게 될건데, 화면의 중앙에서 화면의 크기의 반을 빼줘야 좌측 하단좌표를 구할 수 있기 때문이다.
        LineRenderer lineRenderer = Instantiate(map).GetComponent<LineRenderer>();
        
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2); //좌측 하단
        lineRenderer.SetPosition(1, new Vector2(x + mapSize.x, y) - mapSize / 2); //우측 하단
        lineRenderer.SetPosition(2, new Vector2(x + mapSize.x, y + mapSize.y) - mapSize / 2);//우측 상단
        lineRenderer.SetPosition(3, new Vector2(x, y + mapSize.y) - mapSize / 2); //좌측 상단

    }

}
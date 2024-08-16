using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameStage3
{
    public class ObjectPlacer : MonoBehaviour
    {
        public Camera mainCamera;
        public GameObject objectPrefab; // 이것은 일반적인 GameObject 프리팹입니다
        public Button itemButton;
        
        private TilemapGridSystem gridSystem;

        private void Awake()
        {
            gridSystem = GetComponent<TilemapGridSystem>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPosition = gridSystem.GetGridPosition(mouseWorldPos);

                if (gridSystem.CanPlaceObjectAt(gridPosition))
                {
                    GameObject newObject = Instantiate(objectPrefab);
                    gridSystem.PlaceObject(newObject, gridPosition);
                }
            }
        }
    }
}
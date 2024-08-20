using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Game.GameStage3
{
    [CreateAssetMenu(fileName = "NewTileSet", menuName = "2D/Extended Tile Set")]
    public class ExtendedTileSet : ScriptableObject
    {
        [SerializeField] 
        private int mWidth = 3;
        [SerializeField] 
        private int mHeight = 3;
        [SerializeField] 
        private List<TileData> mTileData = new List<TileData>();
        [SerializeField] 
        private TileType mTileSetType;

        public int Width { get => mWidth; set => mWidth = Mathf.Max(1, value); }
        public int Height { get => mHeight; set => mHeight = Mathf.Max(1, value); }
        public TileType TileSetType { get => mTileSetType; set => mTileSetType = value; }
        
        [Serializable]
        private struct TileData
        {
            public TileBase tile;
        }

        public void Initialize()
        {
            int size = mWidth * mHeight;
            if (mTileData.Count != size)
            {
                mTileData = new List<TileData>(new TileData[size]);
            }
        }

        public TileBase GetTile(int x, int y)
        {
            if (x >= 0 && x < mWidth && y >= 0 && y < mHeight)
            {
                int index = y * mWidth + x;
                return mTileData[index].tile;
            }
            return null;
        }

        public void SetTile(int x, int y, TileBase tile)
        {
            if (x >= 0 && x < mWidth && y >= 0 && y < mHeight)
            {
                // int index = (mHeight - 1 - y) * mWidth + x;
                int index = y * mWidth + x;
                mTileData[index] = new TileData { tile = tile };
            }
        }

        private void OnValidate()
        {
            Initialize();
        }
    }
}
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace Game.GameStage3
{
    [CustomEditor(typeof(ExtendedTileSet))]
    public class ExtendedTileSetEditor : Editor
    {
        private bool showAnimatedTileOptions = false;

        public override void OnInspectorGUI()
        {
            ExtendedTileSet tileSet = (ExtendedTileSet)target;

            EditorGUI.BeginChangeCheck();

            // TileSet 자체 Type 설정
            TileType newTileSetType = (TileType)EditorGUILayout.EnumPopup("Tile Set Type", tileSet.TileSetType);

            int newWidth = EditorGUILayout.IntField("Width", tileSet.Width);
            int newHeight = EditorGUILayout.IntField("Height", tileSet.Height);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tileSet, "Modify Tile Set");
                tileSet.TileSetType = newTileSetType;
                tileSet.Width = newWidth;
                tileSet.Height = newHeight;
                tileSet.Initialize();
            }

            EditorGUILayout.Space();

            // 실제 인스펙터에 보이는 좌표는 2*2일때
            // 0,0 1,0
            // 0,1 1,1
            // 으로 보여져야 하고, 저장은
            // 0,1 1,1
            // 0,0 1,0
            // 로 처리해야 한다. (타일맵 좌표순서와 일치시킴)
            
            for (int y = tileSet.Height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < tileSet.Width; x++)
                {
                    EditorGUI.BeginChangeCheck();
                    var currentTile = tileSet.GetTile(x, y);
                    var newTile = (TileBase)EditorGUILayout.ObjectField(currentTile, typeof(TileBase), false, GUILayout.Width(60));

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(tileSet, "Change Tile");
                        tileSet.SetTile(x, y, newTile);
                    }

                    // AnimatedTile에 대한 추가 옵션 표시
                    if (newTile is AnimatedTile)
                    {
                        if (GUILayout.Button("...", GUILayout.Width(20)))
                        {
                            showAnimatedTileOptions = !showAnimatedTileOptions;
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();

                // AnimatedTile 옵션 표시
                if (showAnimatedTileOptions)
                {
                    EditorGUI.indentLevel++;
                    for (int x = 0; x < tileSet.Width; x++)
                    {
                        var tile = tileSet.GetTile(x, y) as AnimatedTile;
                        if (tile != null)
                        {
                            EditorGUILayout.LabelField($"Tile at ({x}, {y}):");
                            EditorGUI.indentLevel++;
                            tile.m_MinSpeed = EditorGUILayout.FloatField("Animation Speed(Min Speed)", tile.m_MinSpeed);
                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(tileSet);
            }
        }
    }
}
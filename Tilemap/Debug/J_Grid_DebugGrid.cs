#if UNITY_EDITOR
using System.Reflection;
using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace JReact.Tilemaps.Debug
{
    public abstract class J_Grid_DebugGrid<T> : ScriptableObject
        where T : J_Tile
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TransformGenerator _Root;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _autoDebug = false;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _tagId = 2;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract JMapGrid<T> _mapGrid { get; }

        // --------------- COMMANDS --------------- //
        [BoxGroup("Debug", true, true, 100), Button(ButtonSizes.Medium)]
        private void DebugMap()
        {
            ClearMap();

            for (int i = 0; i < _mapGrid.TotalCells; i++)
            {
                int x = i % _mapGrid.Width;
                int y = i / _mapGrid.Width;
                GenerateDebugView(_mapGrid.GetTile(x, y));
            }
        }

        [BoxGroup("Debug", true, true, 100), Button(ButtonSizes.Medium)]
        private void ClearMap() => DestroyImmediate(_Root.ThisTransform?.gameObject);

        // --------------- GENERATION --------------- //
        private void GenerateDebugView(T tile)
        {
            var targetGameObject = new GameObject();
            targetGameObject.transform.SetParent(_Root.ThisTransform);
            targetGameObject.name = $"{tile.CellPosition}_{tile.Ground.TileName}";
            targetGameObject.AddComponent<J_Tile_DebugView>().InjectTile(tile);
            targetGameObject.transform.position = tile.WorldPosition;
            DrawTag(targetGameObject);
        }

        // --------------- TAG --------------- //
        private void DrawTag(GameObject target)
        {
            var largeIcons = GetTextures("sv_label_", string.Empty, 0, 8);
            var icon       = largeIcons[_tagId];
            var egu        = typeof(EditorGUIUtility);
            var flags      = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args       = new object[] { target, icon.image };
            var setIcon = egu.GetMethod("SetIconForObject", flags, null, new Type[] { typeof(UnityEngine.Object), typeof(Texture2D) },
                                        null);

            setIcon.Invoke(null, args);
        }

        private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
        {
            GUIContent[] array = new GUIContent[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
            }

            return array;
        }

        private void OnValidate()
        {
            if (_autoDebug) DebugMap();
        }
    }
}
#endif

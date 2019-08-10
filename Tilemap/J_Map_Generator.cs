using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps
{
    public abstract class J_Map_Generator<T> : MonoBehaviour
        where T : J_Tile
    {
        // --------------- CREATORS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected J_TileRetriever _tileInfoGetter;
        // --------------- UNITY TILEMAPS --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] protected Grid _unityMapGrid;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] protected Tilemap _groundTileMap;

        // --------------- THE MAP GRID --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] protected Vector3Int _startPoint;
        [BoxGroup("Setup", true, true, 0), SerializeField] protected int _width;
        // --------------- ABSTRACT --------------- //
        protected abstract JMapGrid<T> _MapGrid { get; }
        protected abstract iIntArrayGetter _GroundCodes { get; }

        // --------------- GENERATION --------------- //
        [BoxGroup("Test", true, true, 100), Button(ButtonSizes.Medium)]
        public void Generate()
        {
            // --------------- INITIATION --------------- //
           if (_GroundCodes == null || _GroundCodes.Length % _width != 0)
                throw new ArgumentException($"{name} {nameof(_GroundCodes)} is not divisible for width {_width}. Not enough columns");

            var allTiles = new T[_GroundCodes.Length];
            Initiate();

            // --------------- CREATION --------------- //
            for (int i = 0; i < _GroundCodes.Length; i++)
            {
                // --------------- POSITION --------------- //
                Vector3Int position = new Vector3Int { x = i % _width, y = i / _width };
                position += _startPoint;

                // --------------- TILE --------------- //
                var groundTileInfo = _tileInfoGetter.GetItemFromId(_GroundCodes.ArrayCode[i]);
                var tile           = CreateTile(i, position, groundTileInfo);

                // --------------- STORE TILE --------------- //
                groundTileInfo.Add(tile);
                allTiles[i] = tile;
            }

            // --------------- CONFIRM --------------- //
            _MapGrid.InitiateMap(_unityMapGrid, allTiles, _width);
        }

        // --------------- INITIATION --------------- //
        protected virtual void Initiate()
        {
            _tileInfoGetter.ClearTileList();
            _groundTileMap.ClearAllTiles();
        }

        // --------------- ABSTRACT --------------- //
        /// <summary>
        /// specifically creates the tile 
        /// </summary>
        /// <param name="index">the current index of the tile</param>
        /// <param name="position">the position of the tile</param>
        /// <param name="groundTileInfo">the base ground used to draw the map</param>
        /// <returns></returns>
        protected abstract T CreateTile(int index, Vector3Int position, J_TileInfo groundTileInfo);
    }

    public interface iIntArrayGetter
    {
        int[] ArrayCode { get; }
        int Length { get; }
    }
}

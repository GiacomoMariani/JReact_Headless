using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps
{
    [Serializable]
    public class J_Tile
    {
        [HideInInspector] public Action<(J_TileInfo previous, J_TileInfo next)> OnGroundChanged;
        //the ground layer
        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] public J_TileInfo Ground { get; private set; }
        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] protected Tilemap _GroundLayer { get; private set; }
        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] public Vector3Int CellPosition { get; private set; }
        [FoldoutGroup("Tile", false, 5), ReadOnly, ShowInInspector] public Vector2 WorldPosition { get; private set; }

        /// <summary>
        /// tile constructor
        /// </summary>
        public J_Tile(Vector3Int cellPosition, Tilemap groundLayer, J_TileInfo ground)
        {
            (_GroundLayer, CellPosition) = (groundLayer, cellPosition);
            WorldPosition                = groundLayer.GetCellCenterWorld(cellPosition);
            SetGround(ground);
        }

        /// <summary>
        /// changes the ground with the new one
        /// </summary>
        /// <param name="newGround"></param>
        public void SetGround(J_TileInfo newGround)
        {
            // --------------- DRAW --------------- //
            DrawTile(_GroundLayer, newGround);

            // --------------- LOGIC --------------- //
            J_TileInfo oldGround = Ground;
            Ground = newGround;
            OnGroundChanged?.Invoke((oldGround, newGround));
        }

        protected void DrawTile(Tilemap tileMap, J_TileInfo tileInfo)
        {
            Assert.IsNotNull(tileMap,  $"{this} - requires a {nameof(tileMap)}");
            Assert.IsNotNull(tileInfo, $"{this} - requires a {nameof(tileInfo)}");
            tileMap.SetTile(CellPosition, tileInfo.UnityTile);
        }

        public override string ToString() => $"{CellPosition}_{Ground}\nWorld Pos: {WorldPosition}";
    }
}

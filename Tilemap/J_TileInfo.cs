using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JReact.Tilemaps
{
    [CreateAssetMenu(menuName = "Reactive/Tilemap/Tile Data", fileName = "Tile")]
    public class J_TileInfo : J_ReactiveList<J_Tile>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _tileCode;
        public int TileCode => _tileCode;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _tileName;
        public string TileName => _tileName;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Color _tileColor = Color.white;
        public Color TileColor => _tileColor;

        [InfoBox("NULL => Empty tile"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly]
        private TileBase _unityTile;
        public TileBase UnityTile => _unityTile;
    }
}

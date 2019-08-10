#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Tilemaps.Debug
{
    public class J_Tile_DebugView : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_Tile _tile;

        // --------------- TAG INJECTION --------------- //
        public void InjectTile(J_Tile tile) => _tile = tile;
    }
}
#endif

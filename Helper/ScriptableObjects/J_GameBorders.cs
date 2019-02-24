using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// used to set game borders
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Basics/Game Borders", fileName = "GameBorders")]
    public class J_GameBorders : ScriptableObject
    {
        [BoxGroup("Border", true, true, 10), SerializeField]
        public float UpBorder;

        [BoxGroup("Border", true, true, 10), SerializeField]
        public float RightBorder;

        [BoxGroup("Border", true, true, 10), SerializeField]
        public float DownBorder;

        [BoxGroup("Border", true, true, 10), SerializeField]
        public float LeftBorder;
    }
}

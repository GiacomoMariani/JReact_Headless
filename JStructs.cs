using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    //this struct represent the borders of the gameboard
    [System.Serializable]
    public struct GameBorders
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

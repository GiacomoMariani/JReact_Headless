using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// the ngame borders
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Basics/Game Borders", fileName = "GameBorders")]
    public class J_GameBorders : ScriptableObject
    {
        [BoxGroup("Border", true, true, 10), SerializeField] public float UpBorder;
        [BoxGroup("Border", true, true, 10), SerializeField] public float RightBorder;
        [BoxGroup("Border", true, true, 10), SerializeField] public float DownBorder;
        [BoxGroup("Border", true, true, 10), SerializeField] public float LeftBorder;

        /// <summary>
        /// creates border based on given height and width
        /// </summary>
        /// <returns>returns an instanc of border</returns>
        public static J_GameBorders CreateBorders(float height, float width)
        {
            var borders = CreateInstance<J_GameBorders>();
            borders.UpBorder    = height;
            borders.DownBorder  = 0;
            borders.RightBorder = width;
            borders.LeftBorder  = 0;
            return borders;
        }

        /// <summary>
        /// returns the line of the given border
        /// </summary>
        /// <param name="border">the direction of the border</param>
        /// <returns>edge is the position of the border and area is the size</returns>
        public (float edge, Vector2 area) GetBorder(Direction border)
        {
            switch (border)
            {
                case Direction.Up:    return (UpBorder, new Vector2(LeftBorder,    RightBorder));
                case Direction.Right: return (RightBorder, new Vector2(DownBorder, UpBorder));
                case Direction.Down:  return (DownBorder, new Vector2(LeftBorder,  RightBorder));
                case Direction.Left:  return (LeftBorder, new Vector2(DownBorder,  UpBorder));
                case Direction.None:
                default: throw new ArgumentOutOfRangeException(nameof(border), border, null);
            }
        }
    }
}

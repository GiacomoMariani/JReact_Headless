using UnityEngine;

namespace JReact
{
    public class JConstants
    {
        public const float VeryHighFloatTolerance = .5f;
        public const float HighFloatTolerance = .1f;
        public const float GeneralFloatTolerance = .01f;
        public const float LowFloatTolerance = .001f;
        public const float VeryLowFloatTolerance = .0001f;

        public const float HoursInDay = 24f;
        public const float MinutesInHour = 60f;
        public const float SecondsInMinute = 60f;

        public static Vector2 VectorZero = new Vector2(0f, 0f);
        public static Vector2 VectorOne = new Vector2(1f,  1f);
    }
}

using UnityEngine;

namespace JReact.TimeProgress
{
    /// <summary>
    /// catches unity fixed time
    /// </summary>
    public class Unity_FixedTime : iDeltaTime
    {
        public float ThisDeltaTime => Time.fixedDeltaTime;
    }

    /// <summary>
    /// catches unity time
    /// </summary>
    public class Unity_Time : iDeltaTime
    {
        public float ThisDeltaTime => Time.deltaTime;
    }
}

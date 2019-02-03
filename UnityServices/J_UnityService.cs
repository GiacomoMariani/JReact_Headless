using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UnityService
{
    /// <summary>
    /// catch the default unity services
    /// </summary>
    [CreateAssetMenu(fileName = "UnityService", menuName = "Reactive/UnityService")]
    public class J_UnityService : ScriptableObject
    {
        // --------------- TIME --------------- //
        [FoldoutGroup("State - Time", false, 5), ReadOnly, ShowInInspector] private Unity_FixedTime _fixedTime = new Unity_FixedTime();
        [FoldoutGroup("State - Time", false, 5), ReadOnly, ShowInInspector] private Unity_Time _time = new Unity_Time();

        [FoldoutGroup("State - Time", false, 5), ReadOnly, ShowInInspector] public float FixedDeltaTime => _fixedTime.ThisDeltaTime;
        [FoldoutGroup("State - Time", false, 5), ReadOnly, ShowInInspector] public float DeltaTime => _time.ThisDeltaTime;

        // --------------- INPUT --------------- //
        [FoldoutGroup("State - Input", false, 10), ReadOnly, ShowInInspector]
        private Unity_AxisCatcher _axis = new Unity_AxisCatcher();
        public float GetAxis(string axisName) { return _axis.GetAxis(axisName); }
        public float GetAxisRaw(string axisName) { return _axis.GetAxisRaw(axisName); }
    }
}
//J_UnityService

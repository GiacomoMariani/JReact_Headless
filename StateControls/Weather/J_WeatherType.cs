using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl.Weather
{
    /// <summary>
    /// this class represent one possible weather
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Weather/Weather Type")]
    public class J_WeatherType : J_State
    {
        // --------------- SETUP --------------- //
        //chances to have this weather
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(1, 25)] private int _weight;
        public int Weight => _weight;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Vector2 _minutesDurationRange = new Vector2(0.01f, 0.06f);

        // --------------- EVENTS --------------- //
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtStart = new JUnityEvent();
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtEnd = new JUnityEvent();

        // --------------- STATE --------------- //
        //used to show the next time of change
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private float _intervalDuration;

        protected override void ActivateThis()
        {
            base.ActivateThis();
            _unityEvents_AtStart?.Invoke();
        }

        protected override void EndThis()
        {
            _unityEvents_AtEnd?.Invoke();
            base.EndThis();
        }

        /// <summary>
        /// calculates how long the weather will endure
        /// </summary>
        /// <returns>the duration of this weather interval</returns>
        public float CalculateNextInterval()
        {
            _intervalDuration = _minutesDurationRange.GetRandomValue() * JConstants.SecondsInMinute;
            return _intervalDuration;
        }
    }
}

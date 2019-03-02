using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Pool.Roamer
{
    /// <summary>
    /// a wind with a 2d force that might change at intervals
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Roamer/Wind", fileName = "Wind")]
    public class J_Wind : J_Service, iObservable<Vector2>
    {
        #region VALUES AND PROPERTIES
        // --------------- EVENT AND CONSTANT --------------- //
        private event JGenericDelegate<Vector2> OnWindChange;
        private const string COROUTINE_WindTag = "COROUTINE_WindChanger";

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _randomWind = true;
        private bool _notRandomWind => !_randomWind;
        // --------------- STATIC WIND --------------- //
        [ShowIf("_notRandomWind"), BoxGroup("Setup", true, true, 0), SerializeField]
        private Vector2 _desiredSpeed = new Vector2(0.5f, 5f);

        // --------------- RANDOM WIND --------------- //
        //the min and max values for the velocity, the first value
        [ShowIf("_randomWind"), BoxGroup("Setup", true, true, 0), SerializeField]
        private Vector2 _horizontalForceRange = new Vector2(-20f, 20f);
        [ShowIf("_randomWind"), BoxGroup("Setup", true, true, 0), SerializeField]
        private Vector2 _verticalForceRange = new Vector2(-20f, 20f);

        // --------------- CHANGING WIND --------------- //
        [ShowIf("_randomWind", true), BoxGroup("Setup", true, true, 0), SerializeField]
        private bool _windChangeOverTime = true;
        [ShowIf("_windChangeOverTime", true), BoxGroup("Setup", true, true, 0), SerializeField]
        private Vector2 _secondsBeforeChange = new Vector2(0f, 15f);
        [ShowIf("_windChangeOverTime", true), BoxGroup("Setup", true, true, 0), SerializeField]
        private bool _additiveChange;

        // --------------- STATE AND BOOKKEEPING --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Vector2 _windSpeed = new Vector2(5f, 50f);
        public Vector2 WindSpeed => _windSpeed;

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private int _instanceId;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public Direction CurrentDirection
            => _windSpeed.GetDirection();
        #endregion

        #region COMMANDS
        /// <inheritdoc />
        /// <summary>
        /// winds start flowing
        /// </summary>
        protected override void ActivateThis()
        {
            base.ActivateThis();
            _instanceId = GetInstanceID();
            //set the speed
            //calculates a random wind if requested
            if (_randomWind) SetRandomSpeed();
            else SetFixedSpeed();

            //stop if no change is required
            if (!_windChangeOverTime) return;
            //keep changing
            Timing.RunCoroutine(WindChanger(), Segment.LateUpdate, _instanceId, COROUTINE_WindTag);
        }

        /// <inheritdoc />
        /// <summary>
        /// wind stops flowing
        /// </summary>
        protected override void EndThis()
        {
            base.EndThis();
            Timing.KillCoroutines(_instanceId, COROUTINE_WindTag);
            //remove the speed
            _windSpeed.x = 0;
            _windSpeed.y = 0;
            OnWindChange?.Invoke(WindSpeed);
        }
        #endregion

        #region WIND CONTROLS
        private void SetFixedSpeed()
        {
            _windSpeed = _desiredSpeed;
            OnWindChange?.Invoke(WindSpeed);
        }

        private void SetRandomSpeed()
        {
            // --------------- CALCULATING ALL DIRECTION SPEED --------------- //
            //horizontal (additive adds the value instead of just setting it)
            if (_additiveChange) _windSpeed.x += _horizontalForceRange.GetRandomValue();
            else _windSpeed.x                 =  _horizontalForceRange.GetRandomValue();

            //vertical (additive adds the value instead of just setting it)
            if (_additiveChange) _windSpeed.y += _verticalForceRange.GetRandomValue();
            else _windSpeed.y                 =  _verticalForceRange.GetRandomValue();

            //event 
            OnWindChange?.Invoke(WindSpeed);
        }

        //runs the wind
        private IEnumerator<float> WindChanger()
        {
            //stop if not running
            if (!IsActive) yield break;
            //wait
            yield return Timing.WaitForSeconds(_secondsBeforeChange.GetRandomValue());
            //set a new speed speed
            SetRandomSpeed();
            //run again
            Timing.RunCoroutine(WindChanger(), Segment.LateUpdate, _instanceId, COROUTINE_WindTag);
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<Vector2> actionToAdd) { OnWindChange      += actionToAdd; }
        public void UnSubscribe(JGenericDelegate<Vector2> actionToRemove) { OnWindChange -= actionToRemove; }
        #endregion
    }
}

using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// the time counter
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time/Delayed Tick")]
    public class J_Ticker : ScriptableObject, iObservable, iResettable
    {
        #region FIELDS AND PROPERTIES
        private event JAction OnTick;

        [BoxGroup("Setup", true, true, 0), SerializeField] private int _tickInterval = 1;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Segment _desiredSegment = Segment.FixedUpdate;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isRunning;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _objectId = -1;
        #endregion

        #region COMMANDS
        /// <summary>
        /// creates a new timer and starts counting
        /// </summary>
        /// <returns>the new timer created</returns>
        public static J_Ticker CreateNewTimer()
        {
            var timer = ScriptableObject.CreateInstance<J_Ticker>();
            timer.StartTicking();
            return timer;
        }

        //starts the ticker
        public void StartTicking()
        {
            //make sure everything is setup correctly and starts the counting
            JConsole.Log($"{name} starts ticking", J_LogTags.TimeProgress, this);
            if (!SanityChecks()) return;
            //complete the setup
            _objectId  = GetInstanceID();
            _isRunning = true;
            //starts counting
            Timing.RunCoroutine(CountOneTick(), _desiredSegment, _objectId, J_CoroutineTags.COROUTINE_TimerTag);
        }

        //stops the ticker
        public void StopTicking()
        {
            JConsole.Log($"{name} stops ticking", J_LogTags.TimeProgress, this);
            Timing.KillCoroutines(_objectId, J_CoroutineTags.COROUTINE_TimerTag);
            _isRunning = false;
        }
        #endregion

        #region INITIALIZATION
        //make sure this is setup correctly
        private bool SanityChecks()
        {
            Assert.IsFalse(_isRunning, $"{name} is already ticking. Cancel command.");
            Assert.IsTrue(_tickInterval > 0, $"{name} tick requires to be positive. Tick: {_tickInterval}.");
            if (_isRunning) return false;
            return true;
        }
        #endregion

        #region COUNTING
        //this counts a single tick
        private IEnumerator<float> CountOneTick()
        {
            //stop if requested
            if (!_isRunning) yield break;

            //wait the amount of desired ticks
            for (int i = 0; i < _tickInterval; i++)
                yield return Timing.WaitForOneFrame;

            //do not send the event if not required anymore
            if (!_isRunning) yield break;

            //send the event
            if (OnTick != null) OnTick();

            //move ahead to the next tick
            Timing.RunCoroutine(CountOneTick(), _desiredSegment, _objectId, J_CoroutineTags.COROUTINE_TimerTag);
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JAction actionToSend) { OnTick   += actionToSend; }
        public void UnSubscribe(JAction actionToSend) { OnTick -= actionToSend; }
        #endregion

        #region DISABLE AND RESET
        protected virtual void OnDisable() { ResetThis(); }
        public void ResetThis() { if(_isRunning)  StopTicking(); }
        #endregion
    }
}

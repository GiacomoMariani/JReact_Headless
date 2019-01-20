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
    [CreateAssetMenu(menuName = "Reactive/Time/Timer")]
    public class J_Timer : ScriptableObject
    {
        #region FIELDS AND PROPERTIES
        private event JGenericDelegate<float> OnTick;

        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0.05f, 2.5f)] private float _tickLengthInSeconds = 1.0f;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isTicking;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _objectId = -1;
        #endregion

        #region COMMANDS
        /// <summary>
        /// creates a new timer and starts countine
        /// </summary>
        /// <returns>the new timer created</returns>
        public static J_Timer CreateNewTimer()
        {
            var timer = ScriptableObject.CreateInstance<J_Timer>();
            timer.StartCount();
            return timer;
        }

        //starts the counter
        public void StartCount()
        {
            //make sure everything is setup correctly and starts the counting
            JConsole.Log($"{name} starts counting", J_LogTags.TimeProgress, this);
            SanityChecks();
            //complete the setup
            _objectId  = GetInstanceID();
            _isTicking = true;
            //starts counting
            Timing.RunCoroutine(CountOneTick(), Segment.Update, _objectId, J_CoroutineTags.COROUTINE_TimerTag);
        }

        //stops the timer
        public void StopCount()
        {
            JConsole.Log($"{name} stops counting", J_LogTags.TimeProgress, this);
            Timing.KillCoroutines(_objectId, J_CoroutineTags.COROUTINE_TimerTag);
            _isTicking = false;
        }
        #endregion

        #region INITIALIZATION
        //make sure this is setup correctly
        private void SanityChecks()
        {
            Assert.IsFalse(_isTicking, $"{name} is already ticking.");
            Assert.IsTrue(_tickLengthInSeconds <= 0, $"{name} tick requires to be positive. Tick: {_tickLengthInSeconds}.");
            if (_isTicking) return;
        }
        #endregion

        #region COUNTING
        //this counts a single tick
        private IEnumerator<float> CountOneTick()
        {
            //stop if requested
            if (!_isTicking) yield break;

            //count the time before the tick
            var beforeTickTime = UnityEngine.Time.time;
            //wait the tick
            yield return Timing.WaitForSeconds(_tickLengthInSeconds);
            //count the time after the tick
            var afterTickTime = UnityEngine.Time.time;
            //calculate the real passed time
            var realTimePassed = afterTickTime - beforeTickTime;
            //remove the  comment below to check time if required
            //Debug.Log("We've been waiting for " + realTimePassed + " for a tick of " + _tickLengthInSeconds);

            //do not send the event if not required anymore
            if (!_isTicking) yield break;

            //send the event
            if (OnTick != null) OnTick(realTimePassed);

            //move ahead to the next tick
            Timing.RunCoroutine(CountOneTick(), Segment.Update, _objectId, J_CoroutineTags.COROUTINE_TimerTag);
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<float> actionToSend) { OnTick   += actionToSend; }
        public void UnSubscribe(JGenericDelegate<float> actionToSend) { OnTick -= actionToSend; }
        #endregion

        #region DISABLE AND RESET
        protected virtual void OnDisable() { ResetThis(); }
        private void ResetThis() { StopCount(); }
        #endregion
    }
}

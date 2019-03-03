using System;
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
    [CreateAssetMenu(menuName = "Reactive/Time Progress/Delayed Tick", fileName = "Ticker")]
    public class J_Ticker : J_GenericCounter
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _tickInterval = 1;
        #endregion

        #region INITIALIZATION
        /// <summary>
        /// creates a new ticker and starts counting
        /// </summary>
        /// <returns>the new ticker created</returns>
        public static J_Ticker CreateTicker(int framePerTick, Segment desiredSegment = Segment.Update, bool autoStart = true,
                                            bool destroyAtDisable = true)
        {
            if (framePerTick <= 0)
                throw new ArgumentOutOfRangeException($"Cannot create a timer with negative seconds. Received {framePerTick}");

            var ticker = J_GenericCounter.CreateCounter<J_Ticker>(desiredSegment, destroyAtDisable);
            ticker._tickInterval = framePerTick;
            if (autoStart) ticker.Activate();
            return ticker;
        }

        //make sure this is setup correctly
        protected override bool SanityChecks()
        {
            Assert.IsTrue(_tickInterval > 0, $"{name} tick requires to be positive. Tick: {_tickInterval}.");
            return base.SanityChecks();
        }
        #endregion

        #region COUNTING
        //this counts a single tick
        protected override IEnumerator<float> CountOneTick()
        {
            //stop if requested
            if (!IsActive) yield break;

            //count the time before the tick
            float beforeTickTime = Time.time;
            //wait the tick
            for (int i = 0; i < _tickInterval; i++)
                yield return Timing.WaitForOneFrame;

            //count the time after the tick
            float afterTickTime = Time.time;
            //calculate the real passed time
            float realTimePassed = afterTickTime - beforeTickTime;

            //do not send the event if not required anymore
            if (!IsActive) yield break;

            //the event send the tick counted
            SendTickEvent(realTimePassed);

            //move ahead to the next tick
            Timing.RunCoroutine(CountOneTick(), _desiredSegment, _objectId, JCoroutineTags.COROUTINE_CounterTag);
        }
        #endregion
    }
}

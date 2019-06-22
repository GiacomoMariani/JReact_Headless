using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// a counter ticking at time intervals
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time Progress/Timer", fileName = "Timer")]
    public sealed class J_Timer : J_GenericCounter
    {
        #region FIELDS AND PROPERTIES
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0.05f, 2.5f)] private float _tickLengthInSeconds = 1.0f;
        #endregion

        #region INITIALIZATION
        /// <summary>
        /// creates a new timer and starts counting
        /// </summary>
        /// <returns>the new counter created</returns>
        public static J_Timer CreateTimer(float secondsPerTick, Segment desiredSegment = Segment.Update, bool autoStart = true)
        {
            if (secondsPerTick <= 0)
                throw new ArgumentOutOfRangeException($"Cannot create a timer with negative seconds. Received {secondsPerTick}");

            var timer = CreateCounter<J_Timer>(desiredSegment);
            timer._tickLengthInSeconds = secondsPerTick;
            if (autoStart) timer.StartCount();
            return timer;
        }

        //make sure this is setup correctly
        protected override bool SanityChecks()
        {
            Assert.IsTrue(_tickLengthInSeconds > 0, $"{name} tick requires to be positive. Tick: {_tickLengthInSeconds}.");
            return base.SanityChecks();
        }
        #endregion

        #region COUNTING
        //counts a single tick
        protected override IEnumerator<float> CountOneTick()
        {
            float realTimePassed = 0f;

            float beforeTickTime = CurrentRealSeconds;
            while (realTimePassed < _tickLengthInSeconds)
            {
                yield return Timing.WaitForOneFrame;
                realTimePassed = CurrentRealSeconds - beforeTickTime;
            }

            //the event and tick again
            SendTickEvent(realTimePassed);
            Tick();
        }
        #endregion
    }
}

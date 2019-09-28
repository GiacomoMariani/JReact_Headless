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
    public sealed class J_Ticker : J_GenericCounter
    {
        // --------------- FIELDS AND PROPERTIES --------------- // 
        [BoxGroup("Setup", true, true), SerializeField] private int _tickInterval = 1;

        // --------------- INITIALIZATION --------------- //
        /// <summary>
        /// creates a new ticker and starts counting
        /// </summary>
        /// <returns>the new ticker created</returns>
        public static J_Ticker CreateTicker(int framePerTick, Segment desiredSegment = Segment.Update, bool autoStart = true)
        {
            if (framePerTick <= 0)
                throw new ArgumentOutOfRangeException($"Cannot create a timer with negative seconds. Received {framePerTick}");

            var ticker = CreateCounter<J_Ticker>(desiredSegment);
            ticker._tickInterval = framePerTick;
            if (autoStart) ticker.StartCount();
            return ticker;
        }

        //make sure this is setup correctly
        protected override bool SanityChecks()
        {
            Assert.IsTrue(_tickInterval > 0, $"{name} tick requires to be positive. Tick: {_tickInterval}.");
            return base.SanityChecks();
        }

        // --------------- COUNTING --------------- //
        //counts a single tick
        protected override IEnumerator<float> CountOneTick()
        {
            float beforeTickTime = CurrentRealSeconds;

            for (int i = 0; i < _tickInterval; i++) yield return Timing.WaitForOneFrame;

            //calculate the real passed time
            float realTimePassed = CurrentRealSeconds - beforeTickTime;

            //send the event and ticks again
            SendTickEvent(realTimePassed);
            Tick();
        }
    }
}

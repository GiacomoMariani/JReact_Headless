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
    public class J_Ticker : J_GenericCounter
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _tickInterval = 1;
        #endregion

        #region INITIALIZATION
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
            if (!IsRunning) yield break;

            //wait the amount of desired ticks
            for (int i = 0; i < _tickInterval; i++)
                yield return Timing.WaitForOneFrame;

            //do not send the event if not required anymore
            if (!IsRunning) yield break;

            //the event send the tick counted
            SendTickEvent(_tickInterval);

            //move ahead to the next tick
            Timing.RunCoroutine(CountOneTick(), _desiredSegment, _objectId, J_CoroutineTags.COROUTINE_TimerTag);
        }
        #endregion
    }
}

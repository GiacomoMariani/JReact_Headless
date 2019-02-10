using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// a time counter that counts seconds
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time Progress/Timer")]
    public class J_Timer : J_GenericCounter
    {
        #region FIELDS AND PROPERTIES
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0.05f, 2.5f)] private float _tickLengthInSeconds = 1.0f;
        #endregion

        #region INITIALIZATION
        //make sure this is setup correctly
        protected override bool SanityChecks()
        {
            Assert.IsTrue(_tickLengthInSeconds > 0, $"{name} tick requires to be positive. Tick: {_tickLengthInSeconds}.");
            return base.SanityChecks();
        }
        #endregion

        #region COUNTING
        //this counts a single tick
        protected override IEnumerator<float> CountOneTick()
        {
            //stop if requested
            if (!IsRunning) yield break;

            //count the time before the tick
            var realTimePassed = 0f;

            //wait the tick
            while (realTimePassed < _tickLengthInSeconds)
            {
                yield return Timing.WaitForOneFrame;
                realTimePassed += Time.deltaTime;
            }

            //remove the  comment below to check time if required
            //Debug.Log("We've been waiting for " + realTimePassed + " for a tick of " + _tickLengthInSeconds);

            //do not send the event if not required anymore
            if (!IsRunning) yield break;

            //the event send the time passed
            SendTickEvent(realTimePassed);

            //move ahead to the next tick
            Timing.RunCoroutine(CountOneTick(), _desiredSegment, _objectId, JCoroutineTags.COROUTINE_TimerTag);
        }
        #endregion
    }
}

using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// a generic timer
    /// </summary>
    public abstract class J_GenericCounter : ScriptableObject, iObservable<float>, iResettable
    {
        #region FIELDS AND PROPERTIES
        protected event JGenericDelegate<float> OnTick;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] protected Segment _desiredSegment = Segment.Update;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected int _objectId = -1;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsRunning { get; private set; } = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _destroyAtDisable = false;
        #endregion

        #region COMMANDS
        /// <summary>
        /// creates a new counter and starts counting
        /// </summary>
        /// <returns>the new counter created</returns>
        public static T CreateNewTimer<T>(bool destroyAtDisable = true)
            where T : J_GenericCounter
        {
            var timer = ScriptableObject.CreateInstance<T>();
            timer._destroyAtDisable = destroyAtDisable;
            timer.StartCount();
            return timer;
        }

        //starts the counter
        public void StartCount()
        {
            //make sure everything is setup correctly and starts the counting
            JConsole.Log($"{name} starts counting", J_LogTags.TimeProgress, this);
            if (!SanityChecks()) return;
            //complete the setup
            _objectId = GetInstanceID();
            IsRunning = true;
            //starts counting
            Timing.RunCoroutine(CountOneTick(), _desiredSegment, _objectId, J_CoroutineTags.COROUTINE_TimerTag);
        }

        //stops the timer
        public void StopCount()
        {
            JConsole.Log($"{name} stops counting", J_LogTags.TimeProgress, this);
            Timing.KillCoroutines(_objectId, J_CoroutineTags.COROUTINE_TimerTag);
            IsRunning = false;
        }
        #endregion

        #region INITIALIZATION
        //make sure this is setup correctly
        protected virtual bool SanityChecks()
        {
            Assert.IsFalse(IsRunning, $"{name} is already ticking. Cancel command");
            if (IsRunning) return false;
            return true;
        }
        #endregion

        #region COUNTING
        //this counts a single tick
        protected abstract IEnumerator<float> CountOneTick();

        //send the event
        protected void SendTickEvent(float tickValue)
        {
            if (OnTick != null) OnTick(tickValue);
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(JGenericDelegate<float> actionToSend)
        {
            if (!IsRunning) StartCount();
            OnTick += actionToSend;
        }

        public void UnSubscribe(JGenericDelegate<float> actionToSend) { OnTick -= actionToSend; }
        #endregion

        #region DISABLE AND RESET
        protected virtual void OnDisable() { ResetThis(); }

        public virtual void ResetThis()
        {
            if (IsRunning) StopCount();
            if (_destroyAtDisable) Destroy(this);
        }
        #endregion
    }
}

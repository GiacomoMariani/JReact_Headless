using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// a progress, related to time
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time Progress/Progress Event")]
    public class J_Progress : ScriptableObject, jObservable<J_Progress>, iResettable
    {
        #region VALUES AND PROPERTIES
        // --------------- EVENTS RELATED TO PROGRESS --------------- //
        private event Action<J_Progress> OnProgressStart;
        private event Action<J_Progress> OnProgressTick;
        private event Action<J_Progress> OnProgressComplete;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_GenericCounter _counter;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_Identifier _identifier;
        public J_Identifier Identifier => _identifier;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _timeRequiredInSeconds;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float SecondsFromStart { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool Paused { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsRunning { get; private set; }

        // --------------- BOOK KEEPING --------------- //
        [BoxGroup("Book Keeping", true, true, 10), ReadOnly, ShowInInspector] public float ProgressPercentage
            => SecondsFromStart / _timeRequiredInSeconds;
        [BoxGroup("Book Keeping", true, true, 10), ReadOnly, ShowInInspector] public float RemainingSeconds
            => _timeRequiredInSeconds - SecondsFromStart;
        #endregion

        #region SETUP METHODS
        public static T CreateProgress<T>(J_GenericCounter counter = null)
            where T : J_Progress
        {
            var progress                 = CreateInstance<T>();
            if (counter == null) counter = J_Ticker.CreateTicker(1);
            progress._counter = counter;
            return progress;
        }

        //add the identifier when requested
        public void SetIdentifier(J_Identifier identifier)
        {
            if (_identifier != null)
                JLog.Warning($"{name} has already an identifier ({_identifier.name}. Cannot set {identifier.name})",
                             JLogTags.TimeProgress, this);

            if (_identifier == null) _identifier = identifier;
        }
        #endregion

        #region COMMANDS
        /// <summary>
        /// start the progress. Elements may be injected at start
        /// </summary>
        /// <param name="secondsToComplete">time required to complete this progress</param>
        public virtual void StartProgress(float secondsToComplete)
        {
            // --------------- INITIALIZATION --------------- //
            if (!SanityChecks(secondsToComplete)) return;
            ResetValues();

            // --------------- SETUP --------------- //
            _timeRequiredInSeconds = (int) secondsToComplete;

            // --------------- RUN --------------- //
            IsRunning = true;
            StartEvent();
            _counter.Subscribe(AddTimePerTick);
        }

        /// <summary>
        /// stop the timer definitely and reset to its base values
        /// </summary>
        public void StopProgress() { ResetValues(); }

        /// <summary>
        /// adds a fixed amount of seconds to the progress
        /// </summary>
        /// <param name="secondsToAdd">the seconds to be added</param>
        public void AddTime(float secondsToAdd) { SecondsFromStart += secondsToAdd; }

        /// <summary>
        /// starts and stops the progress
        /// </summary>
        /// <param name="inPause">to set the pause</param>
        public void SetPause(bool inPause) { Paused = inPause; }

        /// <summary>
        /// fast finish the progress
        /// </summary>
        [BoxGroup("Test", true, true, 100), Button("FastFinish", ButtonSizes.Medium)]
        public void FastFinish() { ProgressComplete(); }
        #endregion

        #region SETUP
        //make sure all the fields are correct
        private bool SanityChecks(float secondsToComplete)
        {
            Assert.IsNotNull(_counter, $"{name} has not counter. Command canceled.");
            Assert.IsTrue(secondsToComplete > 0,
                          $"{name} requires positive secondsToComplete. Received: {secondsToComplete}.Command canceled.");

            Assert.IsFalse(IsRunning, $"{name} is already started. Command canceled.");
            return secondsToComplete > 0 && !IsRunning && _counter != null;
        }
        #endregion

        #region MAIN EVENTS
        //sends the start event
        [BoxGroup("Debug", true, true, 100), Button("Start Event", ButtonSizes.Medium)]
        private void StartEvent() { OnProgressStart?.Invoke(this); }

        //sends the tick event
        [BoxGroup("Debug", true, true, 100), Button("Tick Event", ButtonSizes.Medium)]
        private void TickEvent() { OnProgressTick?.Invoke(this); }

        //sends the complete event
        [BoxGroup("Debug", true, true, 100), Button("Complete Event", ButtonSizes.Medium)]
        private void CompleteEvent() { OnProgressComplete?.Invoke(this); }
        #endregion

        #region COUNT AND COMPLETION
        //add the time for each tick
        private void AddTimePerTick(float tickDeltaTime)
        {
            //stop if this is not active
            if (Paused) return;

            //add the time to the time passed
            CountSeconds(tickDeltaTime);

            //stop if we reached the end
            if (SecondsFromStart >= _timeRequiredInSeconds) ProgressComplete();
        }

        //used to count the seconds passed
        protected virtual void CountSeconds(float secondsFromCounter)
        {
            SecondsFromStart += secondsFromCounter;
            TickEvent();
        }

        //this is used to start the construction
        private void ProgressComplete()
        {
            Assert.IsTrue(IsRunning, $"{name} only running progress may complete");
            SecondsFromStart = _timeRequiredInSeconds;
            StopTrackingTime();
            CompleteEvent();
        }
        #endregion

        #region SUBSCRIBE EVENTS
        public void SubscribeToStart(Action<J_Progress> actionToSend) { OnProgressStart   += actionToSend; }
        public void UnSubscribeToStart(Action<J_Progress> actionToSend) { OnProgressStart -= actionToSend; }

        public void Subscribe(Action<J_Progress> actionToSend) { OnProgressTick   += actionToSend; }
        public void UnSubscribe(Action<J_Progress> actionToSend) { OnProgressTick -= actionToSend; }

        public void SubscribeToComplete(Action<J_Progress> actionToSend) { OnProgressComplete   += actionToSend; }
        public void UnSubscribeToComplete(Action<J_Progress> actionToSend) { OnProgressComplete -= actionToSend; }
        #endregion

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }

        public virtual void ResetThis()
        {
            ResetValues();
            ResetEvents();
        }

        private void ResetValues()
        {
            //reset the fields
            _timeRequiredInSeconds = 0;
            SecondsFromStart       = 0;
            Paused                 = false;
            //stop if the progress is not counting
            if (IsRunning) StopTrackingTime();
        }

        private void StopTrackingTime()
        {
            _counter.UnSubscribe(AddTimePerTick);
            IsRunning = false;
        }

        private void ResetEvents()
        {
            OnProgressStart    = null;
            OnProgressComplete = null;
            OnProgressTick     = null;
        }
        #endregion
    }

    //an interface for the progress view
    public interface iProgressView
    {
        void InjectProgress(J_Progress progress, J_Identifier identifier = null);
    }
}

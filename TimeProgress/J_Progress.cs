using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// this is an event connected to a time
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time Progress/Progress Event")]
    public class J_Progress : ScriptableObject, iObservable<J_Progress>, iResettable
    {
        #region VALUES AND PROPERTIES
        // --------------- EVENTS RELATED TO PROGRESS --------------- //
        private event JGenericDelegate<J_Progress> OnProgressStart;
        private event JGenericDelegate<J_Progress> OnProgressTick;
        private event JGenericDelegate<J_Progress> OnProgressComplete;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_GenericCounter _timer;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_Identifier _identifier;
        public J_Identifier Identifier => _identifier;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _timeRequiredInSeconds;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public virtual float SecondsFromStart { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool Paused { get; private set; } = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsRunning { get; private set; } = false;

        // --------------- BOOK KEEPING --------------- //
        [BoxGroup("Book Keeping", true, true, 10), ReadOnly, ShowInInspector]
        public float ProgressPercentage => SecondsFromStart / _timeRequiredInSeconds;
        [BoxGroup("Book Keeping", true, true, 10), ReadOnly, ShowInInspector]
        public float RemainingSeconds => _timeRequiredInSeconds - SecondsFromStart;
        #endregion

        #region SETUP METHODS
        public static T InstantiateProgress<T>(bool createTimer = true)
            where T : J_Progress
        {
            var progress                     = CreateInstance<T>();
            if (createTimer) progress._timer = J_GenericCounter.CreateNewTimer<J_Timer>();
            return progress;
        }

        //add the identifier when requested
        public void SetIdentifier(J_Identifier identifier)
        {
            if (_identifier != null)
                JConsole.Warning($"{name} has already an identifier ({_identifier.name}. Cannot set {identifier.name})",
                                 JLogTags.TimeProgress, this);
            if (_identifier == null) _identifier = identifier;
        }

        //make sure we have a valid timer
        public void SetTimer(J_Timer timer)
        {
            if (_timer != null)
                JConsole.Warning($"{name} has already a timer ({_timer.name}. Cannot set {timer.name})",
                                 JLogTags.TimeProgress, this);
            if (_timer == null) _timer = timer;
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
            if (!_timer.IsActive)
            {
                JConsole.Warning($"{_timer.name} on {name} was not running. Force Start.", JLogTags.TimeProgress, this);
                _timer.Activate();
            }

            // --------------- RUN --------------- //
            IsRunning = true;
            StartEvent();
            _timer.SubscribeToWindChange(AddTimePerTick);
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
            Assert.IsNotNull(_timer, $"{name} has not timer. Command canceled.");
            Assert.IsTrue(secondsToComplete > 0,
                          $"{name} requires positive secondsToComplete. Received: {secondsToComplete}.Command canceled.");
            Assert.IsFalse(IsRunning, $"{name} is already started. Command canceled.");
            return secondsToComplete > 0 && !IsRunning && _timer != null;
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
            StopTrackingTime();
            CompleteEvent();
        }
        #endregion

        #region SUBSCRIBE EVENTS
        public void SubscribeToStart(JGenericDelegate<J_Progress> actionToSend) { OnProgressStart   += actionToSend; }
        public void UnSubscribeToStart(JGenericDelegate<J_Progress> actionToSend) { OnProgressStart -= actionToSend; }

        public void SubscribeToWindChange(JGenericDelegate<J_Progress> actionToSend) { OnProgressTick   += actionToSend; }
        public void UnSubscribeToWindChange(JGenericDelegate<J_Progress> actionToSend) { OnProgressTick -= actionToSend; }

        public void SubscribeToComplete(JGenericDelegate<J_Progress> actionToSend) { OnProgressComplete   += actionToSend; }
        public void UnSubscribeToComplete(JGenericDelegate<J_Progress> actionToSend) { OnProgressComplete -= actionToSend; }
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
            _timer.UnSubscribeToWindChange(AddTimePerTick);
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

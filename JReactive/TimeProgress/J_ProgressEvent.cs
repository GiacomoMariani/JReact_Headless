using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// this is an event connected to a time
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time/Progress Event")]
    public class J_ProgressEvent : ScriptableObject, iResettable
    {
        #region VALUES AND PROPERTIES
        // --------------- EVENTS RELATED TO PROGRESS --------------- //
        private event JGenericDelegate<J_ProgressEvent> OnProgressStart;
        private event JGenericDelegate<J_ProgressEvent> OnProgressTick;
        private event JGenericDelegate<J_ProgressEvent> OnProgressComplete;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected J_Timer _timer;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_Identifier _identifier;
        public J_Identifier Identifier => _identifier;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _timeRequiredInSeconds;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public float SecondsFromStart { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _paused = true;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsRunning { get; private set; } = false;

        // --------------- BOOK KEEPING --------------- //
        [BoxGroup("Book Keeping", true, true, 10), ReadOnly, ShowInInspector]
        public float ProgressPercentage => SecondsFromStart / _timeRequiredInSeconds;
        [BoxGroup("Book Keeping", true, true, 10), ReadOnly, ShowInInspector]
        public float RemainingSeconds => _timeRequiredInSeconds - SecondsFromStart;
        #endregion

        #region PRE SETUP
        //add the identifier when requested
        public void SetIdentifier(J_Identifier identifier)
        {
            if (_identifier != null &&
                identifier  != null)
                JConsole.Warning($"{name} has already an identifier ({_identifier.name}. Cannot set {identifier.name})",
                                 J_LogTags.TimeProgress, this);
            if (_identifier == null) _identifier = identifier;
        }

        //make sure we have a valid timer
        public void SetTimer(J_Timer timer)
        {
            if (_timer != null &&
                timer  != null)
                JConsole.Warning($"{name} has already a timer ({_timer.name}. Cannot set {timer.name})",
                                 J_LogTags.TimeProgress, this);
            if (_timer == null &&
                timer  != null) _timer = timer;
        }
        #endregion

        #region COMMANDS
        /// <summary>
        /// start the progress. Elements may be injected at start
        /// </summary>
        /// <param name="secondsToComplete">time required to complete this progress</param>
        /// <param name="createNewTimer">set to true if we want to create/replace the timer</param>
        public void StartProgress(float secondsToComplete, bool createNewTimer = false)
        {
            // --------------- INITIALIZATION --------------- //
            if (!SanityChecks(secondsToComplete)) return;
            ResetValues();

            // --------------- SETUP --------------- //
            if (createNewTimer) _timer = J_Timer.CreateNewTimer();
            _timeRequiredInSeconds = (int) secondsToComplete;

            // --------------- RUN --------------- //
            IsRunning = true;
            ProgressEvent();
            _timer.Subscribe(AddTimePerTick);
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
        public void SetPause(bool inPause) { _paused = inPause; }

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
        private void ProgressEvent()
        {
            if (OnProgressStart != null) OnProgressStart(this);
        }

        //sends the tick event
        [BoxGroup("Debug", true, true, 100), Button("Tick Event", ButtonSizes.Medium)]
        private void TickEvent()
        {
            if (OnProgressTick != null) OnProgressTick(this);
        }

        //sends the complete event
        [BoxGroup("Debug", true, true, 100), Button("Complete Event", ButtonSizes.Medium)]
        private void CompleteEvent()
        {
            if (OnProgressComplete != null) OnProgressComplete(this);
        }
        #endregion

        #region COUNT AND COMPLETION
        //add the time for each tick
        private void AddTimePerTick(float timePassed)
        {
            //stop if this is not active
            if (_paused) return;

            //add the time to the time passed
            SecondsFromStart += timePassed;
            TickEvent();

            //stop if we reached the end
            if (SecondsFromStart >= _timeRequiredInSeconds) ProgressComplete();
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
        public void SubscribeToStart(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressStart   += actionToSend; }
        public void UnSubscribeToStart(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressStart -= actionToSend; }

        public void SubscribeToTick(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressTick           += actionToSend; }
        public void UnSubscribeToProgressTick(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressTick -= actionToSend; }

        public void SubscribeToComplete(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressComplete   += actionToSend; }
        public void UnSubscribeToComplete(JGenericDelegate<J_ProgressEvent> actionToSend) { OnProgressComplete -= actionToSend; }
        #endregion

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }

        public void ResetThis()
        {
            ResetValues();
            ResetEvents();
        }

        private void ResetValues()
        {
            //reset the fields
            _timeRequiredInSeconds = 0;
            SecondsFromStart       = 0;
            _paused                = false;
            //stop if the progress is not counting
            if (IsRunning) StopTrackingTime();
        }

        private void StopTrackingTime()
        {
            _timer.UnSubscribe(AddTimePerTick);
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
        void InjectProgress(J_ProgressEvent progress, J_Identifier identifier = null);
    }
}

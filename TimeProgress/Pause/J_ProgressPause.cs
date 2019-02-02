using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress.Pause
{
    /// <summary>
    /// a progress event with a pause
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Time/Progress With Pause")]
    public class J_ProgressPause : J_Progress
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_PauseEvent _pauseEvent;
        #endregion

        #region PAUSE
        /// <summary>
        /// connect the progress to a pause event
        /// </summary>
        /// <param name="pauseEvent"></param>
        public void InjectPauseEvent(J_PauseEvent pauseEvent)
        {
            _pauseEvent = pauseEvent;
            TrackPause();
        }

        /// <summary>
        /// used to stop tracking a pause
        /// </summary>
        public void RemovePause()
        {
            Assert.IsNotNull(_pauseEvent, $"{name} has no pause event to remove");
            UnTrackPause();
        }

        private void TrackPause()
        {
            if (_pauseEvent.IsPaused &&
                !Paused) SetPause(true);
            _pauseEvent.SubscribeToPauseStart(Pause);
            _pauseEvent.SubscribeToPauseEnd(UnPause);
        }

        private void UnTrackPause()
        {
            if (_pauseEvent == null) return;
            _pauseEvent.UnSubscribeToPauseStart(Pause);
            _pauseEvent.UnSubscribeToPauseEnd(UnPause);
            _pauseEvent = null;
        }

        private void Pause() { SetPause(true); }
        private void UnPause(int item) { SetPause(false); }
        #endregion

        #region OVERRIDES
        //start tracking uses also the pause
        public override void StartProgress(float secondsToComplete)
        {
            // --------------- PAUSE --------------- //
            if (_pauseEvent != null) TrackPause();
            //start tracking
            base.StartProgress(secondsToComplete);
        }

        //reset untrack the pause
        public override void ResetThis()
        {
            if (_pauseEvent) UnTrackPause();
            base.ResetThis();
        }
        #endregion
    }
}

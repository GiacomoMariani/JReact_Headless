using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// abstract class to relate a component to a J_ProgressEvent
    /// </summary>
    public abstract class J_Mono_ProgressView : MonoBehaviour, iProgressView
    {
        #region FIELDS AND PROPERTIES
        //the progress may be set manually of by injection
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] protected J_Progress _progressEvent;
        //this may be left unassigned
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_Identifier _progressId;

        //tracking elements
        [BoxGroup("State", true, true, 5), ReadOnly, SerializeField] private bool _isTracking;
        #endregion

        #region INITIALIZATION
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis() {}

        protected virtual void SanityChecks() {}
        #endregion

        #region INJECTION
        /// <summary>
        /// inject a progress
        /// </summary>
        /// <param name="progress">the progress event</param>
        /// <param name="identifier">the identifier of the progress event</param>
        public void InjectProgress(J_Progress progress, J_Identifier identifier = null)
        {
            //stop if an id has been set and there's no match
            if (_progressId != identifier &&
                _progressId != null) return;

            //set the progress
            if (_isTracking) StopTracking();
            _progressEvent = progress;

            //start tracking directly if the view is active
            if (gameObject.activeSelf) StartTracking();
        }
        #endregion

        #region TRACKING
        private void StartTracking()
        {
            // --------------- SETUP --------------- //
            Assert.IsFalse(_isTracking, $"{gameObject.name} wants to track {_progressEvent.name}, but it is already tracking");
            if (_isTracking) StopTracking();
            _isTracking = true;
            ViewEnabled(_progressEvent);

            // --------------- SUBSCRIBING --------------- //
            _progressEvent.SubscribeToStart(ProgressStart);
            _progressEvent.Subscribe(ProgressUpdate);
            _progressEvent.SubscribeToComplete(ProgressComplete);
        }

        protected virtual void StopTracking()
        {
            if (!_isTracking) return;
            _progressEvent.UnSubscribeToStart(ProgressStart);
            _progressEvent.UnSubscribe(ProgressUpdate);
            _progressEvent.UnSubscribeToComplete(ProgressComplete);
            ViewDisabled(_progressEvent);
            _isTracking = false;
        }
        #endregion

        #region INTERFACE IMPLEMENTATION
        /// <summary>
        /// triggered at progress starts
        /// </summary>
        protected abstract void ProgressStart(J_Progress progress);

        /// <summary>
        /// triggered at each tick
        /// </summary>
        protected abstract void ProgressUpdate(J_Progress progress);

        /// <summary>
        /// triggered at progress complete
        /// </summary>
        protected abstract void ProgressComplete(J_Progress progress);

        protected virtual void ViewEnabled(J_Progress progress) {}
        protected virtual void ViewDisabled(J_Progress progress) {}
        #endregion

        #region LISTENERS
        //start and stop tracking on enable
        private void OnEnable()
        {
            if (_progressEvent != null) StartTracking();
        }

        private void OnDisable() { StopTracking(); }
        #endregion
    }
}

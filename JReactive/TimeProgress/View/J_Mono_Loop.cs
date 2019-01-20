using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// sends events during a loop
    /// </summary>
    public class J_Mono_Loop : MonoBehaviour
    {
        // --------------- ELEMENTS RELATED TO COUTNINT --------------- //
        [BoxGroup("Setup - Count", true, true, 0), SerializeField] private bool _startAtAwake = true;
        [BoxGroup("Setup - Count", true, true, 0), SerializeField] private float _intervalInSeconds = 3.0f;

        // --------------- EVENTS TO SEND --------------- //
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtStart;
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtLoop;
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtEnd;

        // --------------- OPTIONALS - THEY MAY BE AUTO IMPLEMENTED --------------- //
        [BoxGroup("Setup - Optionals", true, true, 10), SerializeField, AssetsOnly] private J_ProgressEvent _progressEvent;
        [BoxGroup("Setup - Optionals", true, true, 10), SerializeField, AssetsOnly] private J_Timer _timer;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 30), ReadOnly] private bool _IsActive => _progressEvent != null && _progressEvent.IsRunning;

        #region INITIALIZATION
        private void Awake()
        {
            ProgressSafeChecks();
            _unityEvents_AtStart.Invoke();
            if (_startAtAwake) StartLoop();
        }

        //make sure the progress event is set
        private void ProgressSafeChecks()
        {
            //create the event if missing
            if (_progressEvent != null)
                _progressEvent = ScriptableObject.CreateInstance<J_ProgressEvent>();

            //add the timer if set
            if (_timer != null) _progressEvent.SetTimer(_timer);
        }
        #endregion

        #region LOOP
        /// <summary>
        /// start the loop
        /// </summary>
        [BoxGroup("Debug", true, true, 100), Button("Start Looping", ButtonSizes.Medium)]
        public void StartLoop()
        {
            //avoid multiple loops
            Assert.IsFalse(_IsActive, $"{gameObject.name} is already looping and cannot start again. Cancel command.");
            if (_IsActive) return;

            JConsole.Log($"Loop starts on {gameObject.name}", J_LogConstants.TimeProgress, this);
            // --------------- PROGRESS SET --------------- //
            _progressEvent.SubscribeToComplete(TriggerThisLoop);
            _progressEvent.StartProgress(_intervalInSeconds);
        }

        //invoke events and loop again
        private void TriggerThisLoop(J_ProgressEvent progress)
        {
            _unityEvents_AtLoop.Invoke();
            _progressEvent.StartProgress(_intervalInSeconds);
        }

        /// <summary>
        /// stop the loop
        /// </summary>
        [BoxGroup("Test", true, true, 100), Button("Stop Looping", ButtonSizes.Medium)]
        public void StopLoop()
        {
            //avoid stop non active loop
            Assert.IsTrue(_IsActive, $"{gameObject.name} is not looping and cannot stop. Cancel command.");
            if (!_IsActive) return;

            JConsole.Log($"Loop stops on {gameObject.name}", J_LogConstants.TimeProgress, this);
            // --------------- COMPLETE LOOP --------------- //
            _progressEvent.UnSubscribeToComplete(TriggerThisLoop);
            _progressEvent.StopProgress();
            _unityEvents_AtEnd.Invoke();
        }
        #endregion

        #region LISTENERS
        //stop on destroy
        private void OnDestroy() { StopLoop(); }
        #endregion
    }
}

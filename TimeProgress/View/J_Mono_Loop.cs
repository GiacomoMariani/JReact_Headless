using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// sends events during a loop
    /// </summary>
    public class J_Mono_Loop : MonoBehaviour, iActivable
    {
        // --------------- ELEMENTS RELATED TO COUTNINT --------------- //
        [BoxGroup("Setup - Count", true, true, 0), SerializeField] private bool _startAtAwake = true;
        [BoxGroup("Setup - Count", true, true, 0), SerializeField] private float _intervalInSeconds = 3.0f;

        // --------------- EVENTS TO SEND --------------- //
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtStart;
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtLoop;
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtEnd;

        // --------------- OPTIONALS - THEY MAY BE AUTO IMPLEMENTED --------------- //
        [BoxGroup("Setup - Optionals", true, true, 10), SerializeField, AssetsOnly] private J_Progress _progressEvent;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 25), ReadOnly, ShowInInspector] public bool IsActive { get; private set; }

        #region INITIALIZATION
        private void Awake()
        {
            ProgressSafeChecks();
            if (_startAtAwake) Activate();
        }

        //make sure the progress event is set
        private void ProgressSafeChecks()
        {
            if (_progressEvent == null)
                _progressEvent = J_Progress.InstantiateProgress<J_Progress>();
        }
        #endregion

        #region LOOP
        /// <summary>
        /// start the loop
        /// </summary>
        [BoxGroup("Debug", true, true, 100), Button("Start Looping", ButtonSizes.Medium)]
        public bool Activate()
        {
            //avoid multiple loops
            Assert.IsFalse(IsActive, $"{gameObject.name} is already looping and cannot start again. Cancel command.");
            if (IsActive) return false;

            // --------------- START --------------- //
            JConsole.Log($"Loop starts on {gameObject.name}", JLogTags.TimeProgress, this);
            IsActive = true;
            _unityEvents_AtStart.Invoke();

            // --------------- PROGRESS SET --------------- //
            Assert.IsTrue(!_progressEvent.IsRunning, $"{gameObject.name} loop progress -{_progressEvent.name}- was already running.");
            _progressEvent.SubscribeToComplete(TriggerThisLoop);
            _progressEvent.StartProgress(_intervalInSeconds);
            return true;
        }

        //invoke events and loop again
        private void TriggerThisLoop(J_Progress progress)
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
            Assert.IsTrue(IsActive, $"{gameObject.name} was not looping. Cancel command.");
            if (!IsActive) return;

            JConsole.Log($"Loop stops on {gameObject.name}", JLogTags.TimeProgress, this);
            Assert.IsTrue(_progressEvent.IsRunning, $"{gameObject.name} loop progress -{_progressEvent.name}- was not running.");
            // --------------- COMPLETE LOOP --------------- //
            _progressEvent.UnSubscribeToComplete(TriggerThisLoop);
            _progressEvent.StopProgress();
            _unityEvents_AtEnd.Invoke();
            IsActive = false;
        }
        #endregion

        #region LISTENERS
        //stop on destroy
        private void OnDestroy() { StopLoop(); }
        #endregion
        public void ResetThis() { StopLoop(); }
    }
}

using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.TimeProgress
{
    /// <summary>
    /// sends events during a loop
    /// </summary>
    public sealed class J_Mono_Loop : MonoBehaviour, iActivable
    {
        // --------------- ELEMENTS RELATED TO COUTNINT --------------- //
        [BoxGroup("Setup - Count", true, true, 0), SerializeField] private bool _startAtAwake = true;

        // --------------- EVENTS TO SEND --------------- //
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtStart;
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtLoop;
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtEnd;

        // --------------- OPTIONALS - THEY MAY BE AUTO IMPLEMENTED --------------- //
        [InfoBox("NULL => will create a timer"), BoxGroup("Setup - Optionals", true, true, 10), SerializeField, AssetsOnly]
        private J_GenericCounter _counter;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 25), ReadOnly, ShowInInspector] public bool IsActive { get; private set; }

        #region INITIALIZATION
        private void Awake()
        {
            Counter();
            if (_startAtAwake) Activate();
        }

        //make sure we have a counter
        private void Counter()
        {
            if (_counter == null)
                _counter = J_Timer.CreateTimer(3f, Segment.FixedUpdate, true);
        }
        #endregion

        #region LOOP
        /// <summary>
        /// start the loop
        /// </summary>
        [BoxGroup("Debug", true, true, 100), Button("Start Looping", ButtonSizes.Medium)]
        public void Activate()
        {
            //avoid multiple loops
            if (IsActive)
            {
                JLog.Warning($"{gameObject.name} is looping and cannot restart. Cancel start.", JLogTags.TimeProgress, this);

                return;
            }

            // --------------- START --------------- //
            JLog.Log($"Loop starts on {gameObject.name}", JLogTags.TimeProgress, this);
            IsActive = true;
            _unityEvents_AtStart.Invoke();

            // --------------- PROGRESS SET --------------- //
            _counter.Subscribe(Loop);
        }

        //invoke events and loop again
        private void Loop(float deltaTime) => _unityEvents_AtLoop.Invoke();

        /// <summary>
        /// stop the loop
        /// </summary>
        [BoxGroup("Test", true, true, 100), Button("Stop Looping", ButtonSizes.Medium)]
        public void End()
        {
            //avoid stop non active loop
            if (!IsActive)
            {
                JLog.Warning($"{gameObject.name} was not looping. Cancel stop.", JLogTags.TimeProgress, this);
                return;
            }

            JLog.Log($"Loop stops on {gameObject.name}", JLogTags.TimeProgress, this);
            Assert.IsTrue(_counter.IsActive, $"{gameObject.name} loop progress -{_counter.name}- was not running.");
            // --------------- COMPLETE LOOP --------------- //
            _counter.UnSubscribe(Loop);
            _unityEvents_AtEnd.Invoke();
            IsActive = false;
        }
        #endregion

        #region LISTENERS
        //stop on destroy
        private void OnDestroy() => End();
        public void ResetThis() => End();
        #endregion
    }
}

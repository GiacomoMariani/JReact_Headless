using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControls
{
    /// <summary>
    /// used to set an order of initialization
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/Ordered Initialization")]
    public class J_OrderedInitialization : JReactiveBool
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State[] _orderedStates;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _currentIndex;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isRunning;

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private J_State _currentState;
        #endregion

        #region INITIALZIATION
        /// <summary>
        /// initialize all states in a given order
        /// </summary>
        public void Initialize()
        {
            JConsole.Log($"{name} start initialization for {_orderedStates.Length} states", JLogTags.State, this);
            if (!ValidInitialization()) return;
            _isRunning    = true;
            _currentIndex = 0;

            InitializeNextState();
        }

        private bool ValidInitialization()
        {
            Assert.IsFalse(CurrentValue, $"{name} is already initialized");
            return !CurrentValue && SanityCheck();
        }

        //checks for complete or initialize the next state
        private void InitializeNextState()
        {
            // --------------- COMPLETE INITIALIZATION --------------- //
            if (_currentIndex >= _orderedStates.Length)
            {
                CompleteInitialization();
                return;
            }

            // --------------- NEXT STATE --------------- //
            _currentState = _orderedStates[_currentIndex];
            Assert.IsNotNull(_currentState , $"{name} has a null state at index {_currentIndex}");
            _currentState.Subscribe(InitializeNextState);
            _currentState.RaiseEvent();
            _currentIndex++;
        }

        private void CompleteInitialization()
        {
            _isRunning   = false;
            CurrentValue = true;
            JConsole.Log($"{name} initialization complete", JLogTags.State, this);
        }
        #endregion

        #region DE INITIALIZATION
        // --------------- DE INITIALIZATION --------------- //
        /// <summary>
        /// de initialize all states in a given order
        /// </summary>
        public void DeInitialize()
        {
            if (!ValidDeInitialization()) return;
            _isRunning    = true;
            _currentIndex = 0;

            DeInitializeNextState();
        }

        private bool ValidDeInitialization()
        {
            Assert.IsTrue(CurrentValue, $"{name} is not initialized");
            return CurrentValue && SanityCheck();
        }

        //checks for complete or de initialize the next state
        private void DeInitializeNextState()
        {
            // --------------- COMPLETE DE INITIALIZATION --------------- //
            if (_currentIndex >= _orderedStates.Length)
            {
                CompleteDeInitialization();
                return;
            }

            // --------------- NEXT STATE --------------- //
            _currentState = _orderedStates[_currentIndex];
            Assert.IsNotNull(_currentState , $"{name} has a null state at index {_currentIndex}");
            _currentState.Subscribe(DeInitializeNextState);
            _currentState.RaiseExitEvent();
            _currentIndex++;
        }

        private void CompleteDeInitialization()
        {
            _isRunning   = false;
            CurrentValue = false;
            JConsole.Log($"{name} de initialization complete", JLogTags.State, this);
        }

        private bool SanityCheck()
        {
            Assert.IsTrue(_orderedStates != null && _orderedStates.Length > 0, $"{name} has no states");
            Assert.IsFalse(_isRunning, $"{name} is already running. Stop initialization");
            return !_isRunning && (_orderedStates == null || _orderedStates.Length <= 0);
        }
        #endregion
    }
}

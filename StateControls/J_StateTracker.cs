using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    /// <summary>
    /// used to track the flow of events to move back to a previous state
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/J State Tracker")]
    public class J_StateTracker : ScriptableObject, iActivable
    {
        #region VALUES AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField, Required, AssetsOnly] private J_SimpleStateControl _stateControl;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _maxStatesToTrack = 5;

        //used to get the previous state
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        public J_State PreviousState
        {
            get
            {
                if (_previousStates.Count == 0) return null;
                return _previousStates[_previousStates.Count - 1];
            }
        }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private List<J_State> _previousStates = new List<J_State>();
        
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; } = false;
        #endregion

        #region INITIALIZATION AND LISTENERS
        public void Activate()
        {
            SanityChecks();
            InitThis();
        }
        
        private void SanityChecks()
        {
            Assert.IsNotNull(_stateControl, $"{name} needs a state control.");
            Assert.IsFalse(IsActive, $"{name} already started, it should not start again.");
        }

        //used to initialize this element
        private void InitThis()
        {
            //ignore if already tracking
            if (IsActive) return;
            _stateControl.Subscribe(ChangeState);
            IsActive = true;
        }
        #endregion

        #region STATE CHANGE PROCESSING
        //called when the state change
        private void ChangeState(J_State previousState, J_State nextState) { AppendStateToPrevious(previousState); }

        //used to stacking the states
        private void AppendStateToPrevious(J_State oldState)
        {
            //if we reached the max states remove the first
            if (_previousStates.Count >= _maxStatesToTrack) _previousStates.RemoveAt(0);
            //then append the last stae
            _previousStates.Add(oldState);
        }
        #endregion

        #region COMMANDS
        /// <summary>
        /// moves the state control to the previous state
        /// </summary>
        public void GoToPreviousState()
        {
            if (NoPreviousStates()) return;

            JConsole.Log($"{name} resets {_stateControl.name} to {PreviousState}", JLogTags.State, this);
            _stateControl.SetNewState(PreviousState);
            _previousStates.RemoveAt(_previousStates.Count - 1);
        }

        //a safecheck to avoid calling this without previous states
        private bool NoPreviousStates()
        {
            if (_previousStates.Count > 0) return false;
            JConsole.Warning($"Currently there are no previous states on {name}. Aborting command.", JLogTags.State, this);
            return true;
        }
        #endregion

        #region DISABLE AND RESET
        protected virtual void OnDisable() { ResetThis(); }

        public void ResetThis()
        {
            if (!IsActive) return;
            _previousStates.Clear();
            _stateControl.UnSubscribe(ChangeState);
            IsActive = false;
        }
        #endregion
    }
}

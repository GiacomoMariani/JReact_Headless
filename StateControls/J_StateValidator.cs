using System;
using System.Collections.Generic;
using UnityEngine;

namespace JReact.StateControl
{
    /// <summary>
    /// a decorator to allow only specific states on a state control
    /// </summary>
    public abstract class J_StateValidator<T> : ScriptableObject
        where T : J_State
    {
        private event Action OnAvailableStateChange;

        // --------------- FIELD AND PROPERTIES --------------- //
        //the state control we want to use
        protected abstract J_StateControl<T> _stateControls { get; }
        //the valid states, empty hashset means we allow all the states
        private HashSet<T> _availableStates = new HashSet<T>();

        // --------------- COMMAND --------------- //
        public void SetNewState(T stateToSet)
        {
            if (!IsAvailable(stateToSet)) return;
            if (_stateControls.IsActive) _stateControls.Activate();
            _stateControls.SetNewState(stateToSet);
        }

        // --------------- QUERY --------------- //
        public bool IsAvailable(T state) => _availableStates.Count == 0 || _availableStates.Contains(state);

        public bool AnyIsAvailable(IEnumerable<T> states) => _availableStates.Count == 0 || _availableStates.Overlaps(states);

        // --------------- SETTINGS --------------- //
        public void SetValidStates(IEnumerable<T> states)
        {
            _availableStates.UnionWith(states);
            OnAvailableStateChange?.Invoke();
        }

        public void ClearStates()
        {
            _availableStates.Clear();
            OnAvailableStateChange?.Invoke();
        }

        public void Subscribe(Action   action) => OnAvailableStateChange += action;
        public void UnSubscribe(Action action) => OnAvailableStateChange -= action;
    }
}

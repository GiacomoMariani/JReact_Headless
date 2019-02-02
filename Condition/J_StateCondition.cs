using JReact.StateControl;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions
{
    /// <summary>
    /// a condition true when a state is active
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Conditions/State")]
    public class J_StateCondition : J_ReactiveCondition
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _passOnEnter = true;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State[] _validStates;
        #endregion

        #region INITIALIZE AND RESET
        protected override void InitializeCheck()
        {
            StateUpdate();
            StartTrackStates();
        }

        protected override void DeInitializeCheck() { StopTrackStates(); }
        #endregion

        private void StateUpdate() { CurrentValue = CheckStates(_passOnEnter); }

        private bool CheckStates(bool wantActive)
        {
            for (int i = 0; i < _validStates.Length; i++)
                if (_validStates[i].IsActive == wantActive) 
                    return true;

            return false;
        }

        #region TRACKERS
        private void StartTrackStates()
        {
            _validStates.SubscribeToAll(StateUpdate);
            for (int i = 0; i < _validStates.Length; i++)
                _validStates[i].SubscribeToExit(StateUpdate);
        }

        private void StopTrackStates()
        {
            _validStates.UnSubscribeToAll(StateUpdate);
            for (int i = 0; i < _validStates.Length; i++)
                _validStates[i].UnSubscribeToExit(StateUpdate);
        }
        #endregion
    }
}

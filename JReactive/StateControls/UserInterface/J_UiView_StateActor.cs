using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControls
{
    /// <summary>
    /// used to change the menu based on the state
    /// </summary>
    public class J_UiView_StateActor : J_Mono_ViewActor
    {
        #region VALUES AND PROPERTIES
        [BoxGroup("State Control", true, true, 0), SerializeField, AssetsOnly, Required]
        protected J_StateControl _mainStateControl;

        //when we want to see this
        [BoxGroup("Controls", true, true, 0), SerializeField] protected J_State[] _validStates;

        //to check the activation of this element
        private bool _isActive;
        protected bool IsActive
        {
            get => _isActive;
            set
            {
                //if we want to set the same value we ignore this
                if (_isActive == value) return;
                //otherwise we set the value
                _isActive = value;
                //then we call the desired method to open or close
                ActivateView(_isActive);
            }
        }
        #endregion

        #region INITIALIZATION
        //set this as requested
        protected override void InitThis() { IsActive = _startsActive; }

        //main checks related to this
        protected virtual void SanityChecks()
        {
            base.SanityChecks();
            //check all states
            for (int i = 0; i < _validStates.Length; i++)
                Assert.IsNotNull(_validStates[i], $"{gameObject.name} has a missing state at index {i}");
        }
        #endregion

        #region LISTENERS
        //update this if the next state is contained here
        protected virtual void StateChange(J_State previousState, J_State nextState)
        {
            IsActive = Array.IndexOf(_validStates, nextState) > -1;
        }

        //start and stop listening to events
        protected virtual void OnEnable()
        {
            StateChange(null, _mainStateControl.CurrentState);
            _mainStateControl.Subscribe(StateChange);
        }

        protected virtual void OnDisable() { _mainStateControl.UnSubscribe(StateChange); }
        #endregion
    }
}

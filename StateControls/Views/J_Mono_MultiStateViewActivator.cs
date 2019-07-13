using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    /// <summary>
    /// used to change the menu based on the state
    /// </summary>
    public sealed class J_Mono_MultiStateViewActivator : MonoBehaviour
    {
        // --------------- VALUES AND PROPERTIES --------------- //

        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _activateWhenEnterState;
        //the views related to this element
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private J_Mono_ViewActivator _view;
        private J_Mono_ViewActivator ThisView
        {
            get
            {
                if (_view == null) _view = GetComponent<J_Mono_ViewActivator>();
                return _view;
            }
        }

        [BoxGroup("State Control", true, true, 0), SerializeField, AssetsOnly, Required]
        protected J_SimpleStateControl _mainStateControl;

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
                ThisView.ActivateView(_isActive);
            }
        }

        // --------------- INITIALIZATION --------------- //
        private void Awake() => SanityChecks();

        private void SanityChecks()
        {
            for (int i = 0; i < _validStates.Length; i++)
                Assert.IsNotNull(_validStates[i], $"{gameObject.name} has a missing state at index {i}");
        }

        // --------------- LISTENERS --------------- //
        private void StateChange((J_State previous, J_State current) transition)
        {
            IsActive = _validStates.ArrayContains(transition.current) == _activateWhenEnterState;
        }

        private void OnEnable()
        {
            _isActive = gameObject.activeSelf;
            if (_mainStateControl.IsActive) StateChange((null, _mainStateControl.CurrentState));
            else _mainStateControl.Subscribe(CheckActivation);

            _mainStateControl.Subscribe(StateChange);
        }

        private void CheckActivation()
        {
            _mainStateControl.UnSubscribe(CheckActivation);
            StateChange((null, _mainStateControl.CurrentState));
        }

        private void OnDisable() => _mainStateControl.UnSubscribe(StateChange);
    }
}

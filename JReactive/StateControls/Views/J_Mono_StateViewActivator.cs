using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControls
{
    /// <summary>
    /// this is used to hide or show a view based on the state
    /// </summary>
    public class J_Mono_StateViewActivator : MonoBehaviour
    {
        #region FIELD AND PROPERTIES
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
        //the desired state
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State _stateToActivate;
        #endregion

        //open or close the views as requested
        private void CloseView() { ThisView.ActivateView(false); }
        private void OpenView() { ThisView.ActivateView(true); }

        #region LISTENERS
        //start and stop tracking on enable
        protected virtual void OnEnable()
        {
            ThisView.ActivateView(_stateToActivate.IsActive);
            StartTracking();
        }

        protected virtual void OnDisable() { StopTracking(); }

        private void StartTracking()
        {
            _stateToActivate.Subscribe(OpenView);
            _stateToActivate.SubscribeToExitEvent(CloseView);
        }

        private void StopTracking()
        {
            _stateToActivate.UnSubscribe(OpenView);
            _stateToActivate.UnSubscribeToExitEvent(CloseView);
        }
        #endregion
    }
}

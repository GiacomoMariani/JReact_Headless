using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControls
{
    /// <summary>
    /// this is used to hide or show a view based on the state
    /// </summary>
    public class J_Mono_StateView : J_Mono_ViewActor
    {
        //the desired state
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State _stateToActivate;

        //open or close the views as requested
        private void CloseView() { ActivateView(false); }
        private void OpenView() { ActivateView(true); }

        #region LISTENERS
        //start and stop tracking on enable
        protected virtual void OnEnable()
        {
            ActivateView(_stateToActivate.IsActivated);
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

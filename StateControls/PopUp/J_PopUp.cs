using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl.PopUp
{
    /// <summary>
    /// a state that sends a pop up
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/PopUp/Reactive Pop Up")]
    public sealed class J_PopUp : J_State
    {
        // --------------- CONSTANTS --------------- //
        private const string DefaultConfirmText = "Confirm";
        private const string DefaultDenyText = "Cancel";

        // --------------- STATE - OPTIONAL --------------- //
        [InfoBox("Null => no connection with state"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly]
        private J_SimpleStateControl _stateControl;

        // --------------- CONTENT --------------- //
        //J_Mono_ReactiveStringText might be used to display this
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _title;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _message;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _confirmButtonText;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _denyButtonText;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 5), SerializeField, AssetsOnly, Required] private J_State _previousState;

        // --------------- ACTIONS --------------- //        
        private JUnityEvent _confirm;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent Confirm
            => _confirm ?? (_confirm = new JUnityEvent());
        private JUnityEvent _deny;
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent Deny
            => _deny ?? (_deny = new JUnityEvent());

        // --------------- SETUP --------------- //
        public void SetupPopUpText(string message, string title = "")
        {
            _message.Current = message;
            _title.Current   = title;
        }

        public void SetupConfirmButton(Action confirmAction, string confirmText = DefaultConfirmText, bool exitStateAfter = true)
        {
            Confirm.RemoveAllListeners();
            Confirm.AddListener(confirmAction.Invoke);
            if (exitStateAfter) Confirm.AddListener(Close);
            _confirmButtonText.Current = confirmText;
        }

        public void SetupDenyButton(Action denyAction, string confirmText = DefaultDenyText, bool exitStateAfter = true)
        {
            Deny.RemoveAllListeners();
            Deny.AddListener(denyAction.Invoke);
            if (exitStateAfter) Deny.AddListener(Close);
            _denyButtonText.Current = confirmText;
        }

        // --------------- OPEN AND CLOSE --------------- //
        public void Open() => Activate();

        public void Close() => End();

        protected override void ActivateThis()
        {
            _previousState = _stateControl.CurrentState;
            base.ActivateThis();
            if (_stateControl              != null &&
                _stateControl.CurrentState != this) _stateControl.SetNewState(this);
        }

        protected override void EndThis()
        {
            base.EndThis();
            if (_stateControl              != null &&
                _stateControl.CurrentState == this &&
                _previousState             != null) _stateControl.SetNewState(_previousState);

            ResetThis();
        }

        public override void ResetThis()
        {
            base.ResetThis();
            Confirm.RemoveAllListeners();
            Deny.RemoveAllListeners();
        }
    }
}

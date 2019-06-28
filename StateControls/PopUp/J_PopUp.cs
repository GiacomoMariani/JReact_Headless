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
        private const string DefaultTitle = "Pop-Up";
        private const string DefaultConfirmText = "Confirm";
        private const string DefaultDenyText = "Cancel";

        // --------------- STATE - OPTIONAL --------------- //
        [InfoBox("Null => no connection with state"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly]
        private J_SimpleStateControl _stateControl;
        [InfoBox("Required if we have state control"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly]
        private J_State _exitState;

        // --------------- CONTENT --------------- //
        //J_Mono_ReactiveStringText might be used to display this
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _title;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _message;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _confirmButtonText;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _denyButtonText;

        // --------------- ACTIONS --------------- //        
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent Confirm { get; private set; }
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent Deny { get; private set; }

        // --------------- SETUP --------------- //
        public void SetupPopUpText(string message, string title = "")
        {
            _message.Current = message;
            _title.Current   = title;
        }

        public void SetupConfirmButton(JUnityEvent confirmAction, string confirmText = DefaultConfirmText, bool exitAfter = true)
        {
            Confirm = confirmAction;
            if (exitAfter) Confirm.AddListener(Close);
            _confirmButtonText.Current = confirmText;
        }

        public void SetupDenyButton(JUnityEvent denyAction, string confirmText = DefaultConfirmText, bool exitAfter = true)
        {
            Deny = denyAction;
            if (exitAfter) Confirm.AddListener(Close);
            _denyButtonText.Current = confirmText;
        }

        // --------------- OPEN AND CLOSE --------------- //
        public void Open() =>  Activate(); 

        public void Close() => End(); 

        protected override void ActivateThis()
        {
            base.ActivateThis();
            if (_stateControl != null) _stateControl.SetNewState(this);
        }

        protected override void EndThis()
        {
            base.EndThis();
            if (_stateControl              != null &&
                _stateControl.CurrentState == this) _stateControl.SetNewState(_exitState);

            ResetThis();
        }

        // --------------- DISABLE AND RESET --------------- //
        public override void ResetThis()
        {
            base.ResetThis();
            //strings
            _title.Current             = DefaultTitle;
            _confirmButtonText.Current = DefaultConfirmText;
            _denyButtonText.Current    = DefaultDenyText;
            _message.ResetThis();
            //actions
            if (Confirm != null)
            {
                Confirm.RemoveAllListeners();
                Confirm = null;
            }

            if (Deny != null)
            {
                Deny.RemoveAllListeners();
                Deny = null;
            }
        }

        private void OnDisable() => ResetThis();
    }
}

using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.PopUp
{
    /// <summary>
    /// a state that sends a pop up
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/PopUp/Reactive Pop Up")]
    public class J_PopUp : J_Service
    {
        #region VALUES AND PROPERTIES
        // --------------- CONSTANTS --------------- //
        private const string DefaultTitle = "Pop-Up";
        private const string DefaultConfirmText = "Confirm";
        private const string DefaultDenyText = "Cancel";

        // --------------- CONTENT --------------- //
        //J_Mono_ReactiveStringText might be used to display this
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _title;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _message;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _confirmButtonText;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _denyButtonText;

        // --------------- ACTIONS --------------- //        
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent Confirm { get; private set; }
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public JUnityEvent Deny { get; private set; }
        #endregion

        #region SETUP
        public void SetupPopUpText(string message, string title = "")
        {
            _message.CurrentValue = message;
            _title.CurrentValue   = title;
        }

        public void SetupConfirmButton(JUnityEvent confirmAction, string confirmText = DefaultConfirmText)
        {
            Confirm                         = confirmAction;
            _confirmButtonText.CurrentValue = confirmText;
        }

        public void SetupDenyButton(JUnityEvent confirmAction, string confirmText = DefaultConfirmText)
        {
            Deny                         = confirmAction;
            _denyButtonText.CurrentValue = confirmText;
        }
        #endregion

        #region OPEN AND CLOSE
        public void Open() { Activate(); }
        public void Close() { End(); }

        public override void End()
        {
            base.End();
            ResetThis();
        }
        #endregion

        #region DISABLE AND RESET
        public override void ResetThis()
        {
            base.ResetThis();
            //strings
            _title.CurrentValue   = DefaultTitle;
            _confirmButtonText.CurrentValue = DefaultConfirmText;
            _denyButtonText.CurrentValue = DefaultDenyText;
            _message.ResetThis();
            //actions
            Confirm = null;
            Deny = null;
        }

        private void OnDisable() { ResetThis(); }
        #endregion
    }
}

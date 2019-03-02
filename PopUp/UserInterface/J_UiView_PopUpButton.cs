using JReact.UiView;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.PopUp
{
    //this is used by a pop up command
    public class J_UiView_PopUpButton : J_UiView_ButtonElement
    {
        // --------------- SETUP --------------- //
        private enum PopUpButtonType { Confirm, Deny }
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_PopUp _popUp;
        [BoxGroup("Setup", true, true, 0), SerializeField] private PopUpButtonType _buttonType;

        // --------------- STATE and BOOKEEPING --------------- //
        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] private JUnityEvent _commandAction
        {
            get
            {
                if (_buttonType == PopUpButtonType.Confirm) return _popUp.Confirm;
                if (_buttonType == PopUpButtonType.Deny) return _popUp.Deny;
                return null;
            }
        }

        // --------------- METHODS --------------- //
        protected override void ButtonCommand() { _commandAction?.Invoke(); }
    }
}

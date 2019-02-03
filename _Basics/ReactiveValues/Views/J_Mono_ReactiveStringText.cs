using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.UiView
{
    /// <summary>
    /// used to show the message on the text
    /// </summary>
    public class J_Mono_ReactiveStringText : J_UiView_TextElement
    {
        //the string with the text
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_ReactiveString _stringScreenMessage;

        #region LISTENERS
        //start and stop tracking on enable
        private void OnEnable()
        {
            SetText(_stringScreenMessage.CurrentValue);
            _stringScreenMessage.Subscribe(SetText);
        }

        private void OnDisable() { _stringScreenMessage.UnSubscribe(SetText); }
        #endregion
    }
}

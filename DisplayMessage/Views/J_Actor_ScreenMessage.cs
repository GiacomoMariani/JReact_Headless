using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// the actor to display a message
    /// </summary>
    public class J_Actor_ScreenMessage : J_Mono_Actor<JMessage>
    {
        #region VALUES AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected J_MessageSender _sender;
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_MessageId[] _desiredTypes;
        #endregion

        #region INITIALIZATION AND SETUP
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_sender, $"{gameObject.name} requires a _messageControl");
        }
        #endregion

        #region SENDER
        private void TrySettingThisElement(JMessage messageSent)
        {
            //publish only desired messages
            if (_desiredTypes.ArrayContains(messageSent.MessageId)) 
                UpdateElement(messageSent);
        }
        #endregion

        #region RESET AND LISTENERS
        protected override void OnEnable()
        {
            base.OnEnable();
            _sender.SubscribeToWindChange(TrySettingThisElement);
        }

        protected virtual void OnDisable() { _sender.UnSubscribeToWindChange(TrySettingThisElement); }
        #endregion
    }
}

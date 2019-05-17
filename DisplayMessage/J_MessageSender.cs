using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// sends the messages
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Screen Message/Sender")]
    public class J_MessageSender : ScriptableObject, jObservable<JMessage>
    {
        #region VALUES AND PROPERTIES
        private event Action<JMessage> OnPublish;

        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_MessageId _defaultIdentifier;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private JMessage _message;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _currentId;
        #endregion

        #region MAIN COMMAND - SEND
        /// <summary>
        /// sends a message on the screen
        /// </summary>
        /// <param name="message">the text to send</param>
        /// <param name="messageId">the type of message</param>
        public void Send(string message, [CanBeNull] J_MessageId messageId = null)
        {
            //use default if no id set
            if (messageId == null) messageId = _defaultIdentifier;

            JLog.Log($"{name} message id {messageId} = {message}", JLogTags.Message, this);
            CreateMessage(message, messageId);

            //send the message
            OnPublish?.Invoke(_message);
        }

        //updates the message to be sent
        private void CreateMessage(string message, J_MessageId messageId)
        {
            _message.MessageContent = message;
            _message.MessageId      = messageId;
            _message.MessageNumber  = _currentId++;
        }
        #endregion

        #region SUBSCRIBERS
        public void Subscribe(Action<JMessage> actionToAdd) { OnPublish      += actionToAdd; }
        public void UnSubscribe(Action<JMessage> actionToRemove) { OnPublish -= actionToRemove; }
        #endregion

        #region TEST
        [BoxGroup("Debug", true, true, 50), SerializeField, AssetsOnly, Required] private J_MessageId _test;

        [BoxGroup("Debug", true, true, 50), Button(ButtonSizes.Medium)]
        private void SendTestMessage() { Send("This is just a test", _test); }
        #endregion
    }

    //the message type
    public struct JMessage
    {
        public int MessageNumber;
        public string MessageContent;
        public J_MessageId MessageId;
    }
}

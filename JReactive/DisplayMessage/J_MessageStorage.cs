using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// stores a number of messages
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Screen Message/Message Storage")]
    public class J_MessageStorage : ScriptableObject
    {
        #region VALUES AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _maxMessages;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_MessageSender _sender;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Queue<JMessage> _messages = new Queue<JMessage>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isRunning;
        #endregion

        #region INITIALIZE AND DEINITIALIZE
        public void Initialize(J_MessageSender messageControls)
        {
            if (messageControls != null) _sender = messageControls;
            if (!ValidStart()) return;
            _sender.Subscribe(AddMessage);
        }

        private bool ValidStart()
        {
            if (_isRunning)
            {
                JConsole.Warning($"{name} is already running. Cancel command", J_LogTags.Message, this);
                return false;
            }

            if (_sender == null)
            {
                JConsole.Error($"{name} has no sender. Cancel command", J_LogTags.Message, this);
                return false;
            }

            return true;
        }

        public void DeInitialize()
        {
            Assert.IsTrue(_isRunning, $"{name} is not running. Cannot deinitialize");
            _sender.UnSubscribe(AddMessage);
            _messages.Clear();
        }
        #endregion

        #region MESSAGE STORAGE
        //stores the message
        private void AddMessage(JMessage message)
        {
            if (_messages.Count > _maxMessages) _messages.Dequeue();
            _messages.Enqueue(message);
        }
        #endregion

        #region RESET AND DISABLE
        //used to reset the item
        private void OnDisable() { ResetThis(); }

        //reset the message in storage
        private void ResetThis()
        {
            if (_isRunning) DeInitialize();
        }
        #endregion
    }
}

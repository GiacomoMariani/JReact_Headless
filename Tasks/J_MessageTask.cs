using JReact.ScreenMessage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// this is a tutorial that sends line of text
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Task/Tutorial Message")]
    public class J_MessageTask : J_Task
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_MessageSender _messageSender;
        //the string related to this tutorial
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _message;

        //the desired type of message
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_MessageId _messageType;
        #endregion

        //sends the message required by this tutorial
        protected override void RunTask() { base.RunTask(); _messageSender.Send(_message, _messageType); }
    }
}

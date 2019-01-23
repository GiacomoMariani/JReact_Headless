using System.Collections.Generic;
using DG.Tweening;
using JReact.UiView;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// pops a message on the screen
    /// </summary>
    public class J_Mono_PoppingMessages : J_Mono_ActorElement<JMessage>
    {
        #region FIELDS AND PROPERTIES
        private const string COROUTINE_PoppingMessagesTag = "COROUTINE_PoppingMessagesTag";

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_UiView_FloatingText _floatingPrefab;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Vector2 _direction;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Color _color;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0.5f, 10.0f)] private float _secondsToComplete = 1.0f;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Ease _messageEase;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 15), ReadOnly]
        private List<(JMessage, J_UiView_FloatingText)> _messageDictionary = new List<(JMessage, J_UiView_FloatingText)>();
        #endregion

        #region INITIALIZATION AND SETUP
        private void Awake() { SanityChecks(); }

        private void SanityChecks() { Assert.IsNotNull(_floatingPrefab, $"{gameObject.name} requires a _simpleTextPrefab"); }
        #endregion

        #region SENDER
        protected override void ActorUpdate(JMessage messageSent)
        {
            //instantiate a new message
            var messageObject = Instantiate(_floatingPrefab, transform);
            //setup the message 
            messageObject.PublishThisMessage(messageSent.MessageContent, _color, _direction, _secondsToComplete, _messageEase);
            //send it to the dictionary
            _messageDictionary.Add((messageSent, messageObject));
            //wait before removal if this is not a permanent message
            Timing.RunCoroutine(WaitThenRemove((messageSent, messageObject)), Segment.FixedUpdate, GetInstanceID(), COROUTINE_PoppingMessagesTag);
        }

        //wait the amount of seconds, then remove the message
        private IEnumerator<float> WaitThenRemove((JMessage, J_UiView_FloatingText) messageTuple)
        {
            //wait then remove
            yield return Timing.WaitForSeconds(_secondsToComplete);
            RemoveOneMessage(messageTuple);
        }

        //used to remove one object
        private void RemoveOneMessage((JMessage, J_UiView_FloatingText) messageTuple)
        {
            var messageView = messageTuple.Item2;
            _messageDictionary.Remove(messageTuple);
            //destroy the message
            Assert.IsNotNull(messageView,
                             $"{gameObject.name} something already destroyed the view of the message {messageTuple.Item1.MessageContent}");
            Destroy(messageView.gameObject);
        }
        #endregion
    }
}

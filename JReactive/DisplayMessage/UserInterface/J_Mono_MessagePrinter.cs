using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.ScreenMessage
{
    /// <summary>
    /// shows a message to be printed on the screen
    /// </summary>
    public class J_Mono_MessagePrinter : J_Mono_ActorElement<JMessage>, iResettable
    {
        #region FIELDS AND PROPERTIS
        private const string COROUTINE_PrinterTag = "COROUTINE_MessagePrinterTag";
        internal event JGenericDelegate<bool> OnPrinting;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private JReactiveString _stringMessage;
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0.01f, 0.5f)] private float _secondsForType = 0.1f;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] private bool _isPrinting = false;
        public bool IsPrinting
        {
            get => _isPrinting;
            private set
            {
                _isPrinting = value;
                if (OnPrinting != null) OnPrinting(value);
            }
        }
        #endregion

        #region INITIALIZATION
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        private void InitThis()
        {
            //injection
            var elementThatRequireThis = GetComponentsInChildren<iInitiator<J_Mono_MessagePrinter>>(true);
            for (int i = 0; i < elementThatRequireThis.Length; i++)
                elementThatRequireThis[i].InjectThis(this);
        }

        private void SanityChecks() { Assert.IsNotNull(_stringMessage, $"({gameObject.name}) needs an element for _currentMessage"); }
        #endregion

        #region OVERRIDES
        protected override void ActorUpdate(JMessage message)
        {
            //reset to make sure it's ready, then print
            ResetThis();
            Timing.RunCoroutine(PrintCurrent(message), Segment.FixedUpdate, _actorElement.MessageNumber, COROUTINE_PrinterTag);
        }
        #endregion

        #region PRINT IMPLEMENTATION
        //print all the chars of the message
        private IEnumerator<float> PrintCurrent(JMessage message)
        {
            //store and validate the message
            var messageToPrint = message.MessageContent;
            if (string.IsNullOrEmpty(messageToPrint)) yield break;
            //reset the print string
            _stringMessage.ResetThis();
            //starts printing, then add a char and wait for the next 
            IsPrinting = true;
            for (int i = 0; i < messageToPrint.Length; i++)
            {
                _stringMessage.CurrentValue += messageToPrint[i];
                yield return Timing.WaitForSeconds(_secondsForType);
            }
            //finish printing
            IsPrinting = false;
        }
        #endregion

        #region MESSAGE COMMANDS
        /// <summary>
        /// used to fast finish to print the message
        /// </summary>
        public void FastComplete()
        {
            Assert.IsTrue(IsPrinting, $"{name} should not call CompletePrinting if it is not printing");
            //reset this and show the entire element
            ResetThis();
            _stringMessage.CurrentValue = _actorElement.MessageContent;
        }
        #endregion

        #region RESET & DISABLE
        protected override void OnDisable()
        {
            base.OnDisable();
            ResetThis();
        }

        public void ResetThis()
        {
            if (!IsPrinting) return;
            Timing.KillCoroutines(_actorElement.MessageNumber, COROUTINE_PrinterTag);
            _stringMessage.ResetThis();
            IsPrinting = false;
        }
        #endregion
    }
}

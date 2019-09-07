using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// this is used to setup anything related to an event
    /// </summary>
    public abstract class J_Mono_EventCallback : MonoBehaviour
    {
        //the states we want to call the event
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Event _event;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        protected virtual void SanityChecks() => Assert.IsNotNull(_event, $"{gameObject.name} requires a {nameof(_event)}");

        protected virtual void InitThis() { _event.Subscribe(CallEvent); }

        // --------------- IMPLEMENTATION --------------- //
        /// <summary>
        /// the event we method to happen when the event is called
        /// </summary>
        protected abstract void CallEvent();

        // --------------- DESTROY --------------- //
        protected virtual void OnDestroy() { _event.UnSubscribe(CallEvent); }
    }
}

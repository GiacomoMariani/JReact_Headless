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
        [BoxGroup("State", true, true, 0), SerializeField, AssetsOnly, Required] private J_Event _stateEvent;

        #region INITIALIZATION
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        protected virtual void SanityChecks()
        { Assert.IsNotNull(_stateEvent, $"This object ({gameObject.name}) needs an element for the value _stateEvent"); }

        protected virtual void InitThis() { _stateEvent.Subscribe(CallEvent); }
        #endregion

        /// <summary>
        /// the event we method to happen when the event is called
        /// </summary>
        protected abstract void CallEvent();

        #region LISTENERS
        protected virtual void OnDestroy() { _stateEvent.UnSubscribe(CallEvent); }
        #endregion
    }
}
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// this class is similar to J_Mono_EventCallback, but it is not used as abstract
    /// it can be used to send directly further simple commands from the editor
    /// use J_Mono_EventCallback if you require an abstract class
    /// </summary>
    public class J_Mono_EventCaller : MonoBehaviour
    {
        #region FIELD AND PROPERTIES
        //the possible condition to cancel the event
        [BoxGroup("Setup", true, true, 5), SerializeField, AssetsOnly] private J_ReactiveBool[] _conditions;
        //if we desire to launch this at startup
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _launchAtAwake;
        //the events where we want to send this
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Event[] _event;
        //what we want to send
        [BoxGroup("Setup", true, true, 0), SerializeField] private JUnityEvent _actions;
        #endregion

        #region INITIALIZATION
        private void Awake() { InitThis(); }

        private void InitThis()
        {
            //call the event at startup if requested
            if (_launchAtAwake) CallEvent();
            //subscribe to the following events
            _event.SubscribeToAll(CallEvent);
        }
        #endregion

        /// <summary>
        /// the event we method to happen when the event is called
        /// </summary>
        private void CallEvent()
        {
            //STEP 1 - check if all save conditions are met
            for (int i = 0; i < _conditions.Length; i++)
            {
                Assert.IsNotNull(_conditions[i], $"The save condition at index {name} of {i} is null");
                if (_conditions[i].Current) continue;
                JLog.Log($"{name} - save canceled. Condition not met: {_conditions[i].name}", JLogTags.EventTag, this);
                return;
            }

            //STEP 2 - if conditions are met send the event
            _actions.Invoke();
        }

        #region LISTENERS
        private void OnDestroy() { _event.UnSubscribeToAll(CallEvent); }
        #endregion
    }
}

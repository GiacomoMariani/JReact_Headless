using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions
{
    /// <summary>
    /// checks ia an event has been sent
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Conditions/Event", fileName = "EVENT_Condition")]
    public class J_EventCondition : J_ReactiveCondition
    {
        #region FIELDS AND PROPERTIES
        //these events set the condition to true
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Event[] _trueEvents;
        //these events set the condition to false
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_Event[] _falseEvents;
        //used to auto reset after a check
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _autoReset = false;
        #endregion

        #region INITIALIZE AND RESET
        protected override void StartCheckingCondition() { StartTrackEvents(); }

        protected override void StopCheckingCondition() { StopTrackEvents(); }
        #endregion

        #region ACTIONS AND TRACKING
        private void SetAsFalse()
        {
            if (CurrentValue) CurrentValue = false;
        }

        private void SetAsTrue()
        {
            if (!CurrentValue) CurrentValue = true;
            if (_autoReset) CurrentValue    = false;
        }

        private void StartTrackEvents()
        {
            _trueEvents.SubscribeToAll(SetAsTrue);
            _falseEvents.SubscribeToAll(SetAsFalse);
        }

        private void StopTrackEvents()
        {
            _trueEvents.UnSubscribeToAll(SetAsTrue);
            _falseEvents.UnSubscribeToAll(SetAsFalse);
        }
        #endregion
    }
}

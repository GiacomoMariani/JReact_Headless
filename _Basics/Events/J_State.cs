using Sirenix.OdinInspector;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// this class is a scriptable object that contains a state that might be injected into any
    /// script who require to follow this state. It acts just like a JSimple event, but it has also
    /// an exit event and not just an enter event
    /// this script represent a game state
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/J Reactive State")]
    public class J_State : J_Event, iStateObservable, iResettable
    {
        //the main event to set the new state
        private event JAction OnExitEvent;

        //to check if this state is active
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; } = false;

        //raise event also activate the state
        public override void RaiseEvent()
        {
            Assert.IsFalse(IsActive, $"{name} was already active");
            if (IsActive) return;
            IsActive = true;
            base.RaiseEvent();
        }

        //this is the property we want to track
        [ButtonGroup("State trigger", 200), Button("Raise Exit Event", ButtonSizes.Medium)]
        public virtual void RaiseExitEvent()
        {
            Assert.IsTrue(IsActive, $"{name} was not active");
            if (!IsActive) return;
            IsActive = false;
            OnExitEvent?.Invoke();
        }

        //a way to subscribe and unsubscribe to the exit event
        public void SubscribeToExit(JAction actionToSend) { OnExitEvent   += actionToSend; }
        public void UnSubscribeToExit(JAction actionToSend) { OnExitEvent -= actionToSend; }

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }

        public virtual void ResetThis()
        {
            if (IsActive) RaiseExitEvent();
        }
        #endregion
    }
}

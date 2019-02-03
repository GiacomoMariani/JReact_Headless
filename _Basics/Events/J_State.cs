using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// a state with an Activate method from J_Event, and with an exit event
    /// this script represents usually a game state or a task
    /// IMPORTANT CONSIDERATION
    /// => if this is used as a STATE something external should call Activate and End
    /// => if this is used as a TASK the external may call Activate, but End should be call internally
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/Reactive State")]
    public class J_State : J_Event, iStateObservable, iActivable
    {
        //the main event to set the new state
        private event JAction OnExitEvent;

        //to check if this state is active
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; } = false;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string Name => name;

        public static J_State CreateInstance(string nameToSet)
        {
            var state = CreateInstance<J_State>();
            state.name = nameToSet;
            return state;
        }
        
        //raise event also activate the state
        public override void Activate()
        {
            Assert.IsFalse(IsActive, $"{name} was already active");
            if (IsActive) return;
            IsActive = true;
            base.Activate();
        }

        //this is the property we want to track
        [ButtonGroup("State trigger", 200), Button("Raise Exit Event", ButtonSizes.Medium)]
        public virtual void End()
        {
            Assert.IsTrue(IsActive, $"{name} was not active");
            if (!IsActive) return;
            IsActive = false;
            OnExitEvent?.Invoke();
        }

        //a way to subscribe and unsubscribe to the exit event
        public void SubscribeToEnd(JAction actionToSend) { OnExitEvent   += actionToSend; }
        public void UnSubscribeToEnd(JAction actionToSend) { OnExitEvent -= actionToSend; }

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }

        public virtual void ResetThis()
        {
            if (IsActive) End();
        }
        #endregion
    }
}

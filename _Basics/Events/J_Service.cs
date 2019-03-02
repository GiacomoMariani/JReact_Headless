using Sirenix.OdinInspector;
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
    public class J_Service : J_Event, iStateObservable, iActivable
    {
        private event JAction OnExitEvent;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string Name => name;

        public static J_Service CreateState(string nameToSet)
        {
            var state = CreateInstance<J_Service>();
            state.name = nameToSet;
            return state;
        }

        /// <summary>
        /// activates the service
        /// </summary>
        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)]
        public override void Activate()
        {
            Assert.IsFalse(IsActive, $"{name} was already active");
            if (IsActive) return;
            IsActive = true;
            base.Activate();
        }

        /// <summary>
        /// ends the service
        /// </summary>
        [ButtonGroup("Commands", 200), Button("End", ButtonSizes.Medium)]
        public virtual void End()
        {
            Assert.IsTrue(IsActive, $"{name} was not active");
            if (!IsActive) return;
            IsActive = false;
            OnExitEvent?.Invoke();
        }

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

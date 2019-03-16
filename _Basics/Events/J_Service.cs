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
    public class J_Service : ScriptableObject, iStateObservable, iActivable
    {
        private event JAction OnEnter;
        private event JAction OnExit;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public string Name => name;

        public static J_Service CreateService(string nameToSet)
        {
            var state = CreateInstance<J_Service>();
            state.name = nameToSet;
            return state;
        }

        /// <summary>
        /// activates the service
        /// </summary>
        [ButtonGroup("Commands", 200), Button("Activate", ButtonSizes.Medium)]
        public void Activate()
        {
            if (IsActive)
            {
                JConsole.Warning($"{name} was already active. Resetting.", JLogTags.Task, this);
                ResetThis();
            }

            IsActive = true;
            ActivateThis();
        }

        //the specific implementation of activate
        protected virtual void ActivateThis() { OnEnter?.Invoke(); }

        /// <summary>
        /// ends the service
        /// </summary>
        [ButtonGroup("Commands", 200), Button("End", ButtonSizes.Medium)]
        public virtual void End()
        {
            Assert.IsTrue(IsActive, $"{name} was not active. Cancel command.");
            if (!IsActive) return;
            EndThis();
            IsActive = false;
        }

        //the specific implementation of activate
        protected virtual void EndThis() { OnExit?.Invoke(); }

        public void Subscribe(JAction actionToSubscribe) { OnEnter   += actionToSubscribe; }
        public void UnSubscribe(JAction actionToSubscribe) { OnEnter -= actionToSubscribe; }

        public void SubscribeToEnd(JAction actionToSend) { OnExit   += actionToSend; }
        public void UnSubscribeToEnd(JAction actionToSend) { OnExit -= actionToSend; }

        #region DISABLE AND RESET
        private void OnDisable() { ResetThis(); }

        public virtual void ResetThis()
        {
            if (IsActive) End();
        }
        #endregion
    }
}

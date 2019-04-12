/*
 * STRATEGY:
 * This scriptable object acts as a state machine, and offers an architecture that relies on Dependency 
 * Injection and Observer pattern. 
 * 
 * COMMANDS AND REGISTERS
 * It receives state changes through the SetNewState method that offers the basics for the Command pattern.
 * It can be injected to scripts that require to follow state changes that may register/subscribe to OnStateTransition 
 *
 * SANITY CHECKS
 * This script keeps checking if the commands are legit, using unity built in assertions.
 *
 * SIDE NOTE
 * This script is using Odin Inspector for better visualization on the unity editor.
 */

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    public class J_StateControl<T> : J_Service, iObservable<(T previous, T current)>
        where T : J_State
    {
        #region FIELDS AND PROPERTIES
        // --------------- MAIN EVENTS AND DELEGATES --------------- //
        private event Action<(T previous, T current)> OnStateTransition;

        // --------------- VALID STATES --------------- //
        /* These are used just a sanity check, to make sure we are implementing the correct states */
        [Title("Setup", "Starts with this state"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly,
         Required]
        protected T _firstState;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] protected T[] _validStates;

        // --------------- CURRENT SITUATION --------------- //
        /* The following items are used to track the current situation */

        [Title("State", "The current situation"), FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private T _currentState;
        public T CurrentState
        {
            get => _currentState;
            private set
            {
                // --------------- EXIT EVENT --------------- //
                //send exit event of the previous event
                Assert.IsNotNull(CurrentState, $"{name} is trying to exit from a null state.");
                CurrentState.End();

                // --------------- VALUE SET --------------- //
                //set the value and raise the event of the next state
                Assert.IsNotNull(value, $"{name} is trying to set a null state.");
                _currentState = value;

                // --------------- ENTER EVENT--------------- //
                CurrentState.Activate();
            }
        }
        #endregion FIELDS AND PROPERTIES

        #region INSTANTIATION
        public static J_StateControl<T> Create(T[] states, T firstState, bool initialize = true)
        {
            var stateControl = CreateInstance<J_StateControl<T>>();
            stateControl._validStates = states;
            stateControl._firstState  = firstState;
            if (initialize) stateControl.Activate();
            return stateControl;
        }

        public static J_StateControl<T> FromTemplate(J_StateControl<T> template, bool initialize = true)
        {
            int length = template._validStates.Length;
            var states = new T[length];
            for (int i = 0; i < length; i++)
                states[i] = J_State.Copy(template._validStates[i]);

            T firstState = J_State.Copy(template._firstState);

            return Create(states, firstState, initialize);
        }
        #endregion INSTANTIATION

        #region ACTIVATION
        // sets the first state of the game
        protected override void ActivateThis()
        {
            base.ActivateThis();
            Assert.IsNotNull(_firstState, $"Please set a first state to validate the controls on: {name}");
            SetStateSanityChecks(_firstState);
            _currentState = _firstState;
            _firstState.Activate();
            JLog.Log($"Initialization completed on {name} with {_validStates.Length} states.", JLogTags.State, this);
        }

        protected override void EndThis()
        {
            if (CurrentState != null) CurrentState.End();
            _currentState = null;
            base.EndThis();
        }
        #endregion ACTIVATION

        #region MAIN CONTROL
        /// <summary>
        /// the main command of this class. This is used to change the state into another most of the
        /// logic will be handled by the property CurrentState
        /// </summary>
        /// <param name="stateToSet">the state we want to set</param>
        public void SetNewState(T stateToSet)
        {
            // --------------- PRE COMMAND CHECKS --------------- //
            if (StateAlreadySet(stateToSet)) return;
            SetStateSanityChecks(stateToSet);

            JLog.Log($"{name} from {(CurrentState != null ? CurrentState.name : "null")} to {stateToSet.name}.",
                     JLogTags.State, this);

            // --------------- COMMAND PROCESSING --------------- //
            T previous = CurrentState;
            CurrentState = stateToSet;
            OnStateTransition?.Invoke((previous, stateToSet));
        }

        private void SetStateSanityChecks(T stateToSet)
        {
            Assert.IsNotNull(stateToSet, $"{name} is trying to set a null state");
            Assert.IsTrue(_validStates.ArrayContains(stateToSet),
                          $"The state {stateToSet} is not in the of valid states of {name}. List{_validStates.PrintAll()}.");
        }

        //to avoid setting the same state again
        private bool StateAlreadySet(T stateToSet)
        {
            if (stateToSet != CurrentState) return false;

            JLog.Warning($"{name} wants to set {stateToSet.name}, but it is already the current state",
                         JLogTags.State, this);

            return true;
        }
        #endregion MAIN CONTROL
        
        #region SUBSCRIBE METHODS
        //the following methods are used to subscribe/register to the transition event. they act like the observer pattern
        public void Subscribe(Action<(T previous, T current)> action)
        {
            if (!IsActive) Activate();
            OnStateTransition += action;
        }

        public void UnSubscribe(Action<(T previous, T current)> action) { OnStateTransition -= action; }

        public void SubscribeToStateChange(Action<(T previous, T current)> action) => Subscribe(action);
        public void UnSubscribeToStateChange(Action<(T previous, T current)> action) => UnSubscribe(action);
        #endregion SUBSCRIBE METHODS
    }
}

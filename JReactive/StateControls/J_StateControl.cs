/*
 * STRATEGY:
 * This scriptable object acts as a state machine, and offers an architecture that relies on Dependency 
 * Injection and Observer pattern. 
 * 
 * COMMANDS AND REGISTERS
 * It receives state changes through the SetNewState method that offers the basics for the Command pattern.
 * It can be injected to scripts that require to follow state changes that may register/subscribe to OnStateTransition 
 * 
 * INITIALIZATION
 * As a bonus it also handles initialization, to make sure we can run some items in a given order at the
 * initialization of the game when the Awake and Start methods are not suited to grant a specific order
 *
 * SANITY CHECKS
 * This script keeps checking if the commands are legit, using unity built in assertions.
 *
 * SIDE NOTE
 * This script is using Odin Inspector for better visualization on the unity editor.
 */

using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControls
{
    /// <summary>
    /// This class implements a state machine sending event to be tracked by other scripts using 
    /// Dependency Injection (we may inject just the desired state or this entire state control)
    /// EXAMPLES OF USAGE: menu flow and transitions, weather system, simple artificial intelligence,
    /// but also friend invitation or any other element that may be tracked with a state machine
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/J State Control")]
    public class J_StateControl : ScriptableObject, iResettable
    {
        #region FIELDS AND PROPERTIES
        // --------------- MAIN EVENTS AND DELEGATES --------------- //
        //this is the event that tracks the transition from one state to one other
        public delegate void StateTransition(J_State previousState, J_State nextState);
        private event StateTransition OnStateTransition;

        // --------------- INITIALIZATION --------------- //
        /* These are simple scriptable objects with just one event that we can inject in any script,
         * so to be triggered at a specific order */

        //the event to be sent before initialization
        [FoldoutGroup("Initialization", true, 0), SerializeField] private J_Event[] _beforeInitialization;
        //an event to be sent after initialization
        [FoldoutGroup("Initialization", true, 0), SerializeField] private J_Event[] _afterInitialization;
        //a bool to track initialization
        [FoldoutGroup("Initialization", true, 0), ReadOnly, ShowInInspector] private bool _initializeBool;
        public bool IsInitialized => _initializeBool;

        // --------------- VALID STATES --------------- //
        /* These are used just a sanity check, to make sure we are implementing the correct states */

        //a list of the valid states
        [Title("States", "The states offered by this control")]
        [BoxGroup("States", true, true, 5), SerializeField]
        private J_State[] _validStates;

        // --------------- CURRENT SITUATION --------------- //
        /* The following items are used to track the current situation */

        [Title("Current", "The current situation")]
        //to check if interaction is available, we can disable it for example during cutscenes
        [BoxGroup("Current", true, true, 10), ReadOnly, ShowInInspector]
        public bool InteractionAvailable => true;

        //the current state, with an available getter, to make sure anyone can check it
        //this property is also used to send the main events
        [BoxGroup("Current", true, true, 10), ReadOnly, ShowInInspector] private J_State _currentState;
        public J_State CurrentState
        {
            get => _currentState;
            private set
            {
                // --------------- EXIT EVENT --------------- //
                //send exit event of the previous event
                Assert.IsNotNull(CurrentState, $"Trying to exit from a null state from {name}.");
                CurrentState.RaiseExitEvent();

                // --------------- VALUE SET --------------- //
                //set the value and raise the event of the next state
                Assert.IsNotNull(value, $"We're trying to set a null state on {name}.");
                _currentState = value;

                // --------------- ENTER EVENT--------------- //
                CurrentState.RaiseEvent();
            }
        }
        #endregion FIELDS AND PROPERTIES

        #region INITIALIZATION
        /// <summary>
        /// during initialization we trigger all the events used to setup game logic.
        /// This is also used to set the first state of the game
        /// </summary>
        /// <param name="initialState">the first state we want to set</param>
        public void Initialize(J_State initialState)
        {
            // --------------- BEFORE INITIALIZATION --------------- //
            //send all elements that require to be setup before the first state, if needed
            if (_beforeInitialization != null) SendBeforeInitializationEvents();

            // --------------- INITIALIZATION --------------- //
            //call the first state
            Assert.IsNotNull(initialState, $"Please set a first state to validate the controls on: {name}");
            initialState.RaiseEvent();
            //set the state without sending any event
            _currentState   = initialState;
            _initializeBool = true;

            // --------------- AFTER INITIALIZATION --------------- //
            //confirm he initialization with the event, if anything require setup after initialization
            if (_afterInitialization != null) SendAfterInitializationEvents();
            HelperConsole.DisplayMessage($"Initialization completed on {name} with {_validStates.Length} states.",
                                         J_DebugConstants.Debug_State);
        }

        // sends all the events we want to have before the initialization
        private void SendBeforeInitializationEvents()
        {
            //we send all events
            for (int i = 0; i < _beforeInitialization.Length; i++)
                _beforeInitialization[i].RaiseEvent();
        }

        // sends all the events happening after the initialization
        private void SendAfterInitializationEvents()
        {
            for (int i = 0; i < _afterInitialization.Length; i++)
                _afterInitialization[i].RaiseEvent();
        }
        #endregion INITIALIZATION

        #region MAIN CONTROL
        /// <summary>
        /// the main command of this class. This is used to change the state into another most of the
        /// logic will be handled by the property CurrentState
        /// </summary>
        /// <param name="stateToSet">the state we want to set</param>
        public void SetNewState(J_State stateToSet)
        {
            // --------------- PRE COMMAND CHECKS --------------- //
            if (StateAlreadySet(stateToSet)) return;
            SetStateSanityChecks(stateToSet);

            HelperConsole.DisplayMessage($"{name} from {(CurrentState != null ? CurrentState.name : "null")} to {stateToSet.name}.",
                                         J_DebugConstants.Debug_State);

            // --------------- COMMAND PROCESSING --------------- //
            CurrentState = stateToSet;
            if (OnStateTransition != null) OnStateTransition(CurrentState, stateToSet);
        }

        //make sure the command is setup correctly
        private void SetStateSanityChecks(J_State stateToSet)
        {
            Assert.IsTrue(_initializeBool,
                          $"New states can be set only after initialization. Message from: {name}. Requested state: {stateToSet.name}");
            Assert.IsTrue(Array.IndexOf(_validStates, stateToSet) > -1,
                          $"The state {stateToSet} is not in the of valid states of {name}. List{_validStates.PrintAll()}.");
        }

        //to avoid setting the same state again
        private bool StateAlreadySet(J_State stateToSet)
        {
            if (stateToSet == CurrentState)
            {
                HelperConsole.DisplayWarning($"{name} wants to set {stateToSet.name}, but it is already the current state",
                                             J_DebugConstants.Debug_State);
                return true;
            }

            return false;
        }
        #endregion MAIN CONTROL

        #region SUBSCRIBE METHODS
        /// <summary>
        /// the following methods are used to subscribe/register to the transition event. they act like the observer pattern
        /// </summary>
        /// <param name="actionToSend">the action/handler to be sent at the event</param>
        public void Subscribe(StateTransition actionToSend) { OnStateTransition += actionToSend; }

        public void UnSubscribe(StateTransition actionToSend) { OnStateTransition -= actionToSend; }
        #endregion SUBSCRIBE METHODS

        #region DISABLE AND RESET
        /// <summary>
        /// reset methods set OnDisable, as for Unity Scriptable objects
        /// </summary>
        protected virtual void OnDisable() { ResetThis(); }

        /// <summary>
        /// we reset the initialization and we remove the current state using the field instead of the property
        /// </summary>
        public virtual void ResetThis()
        {
            _initializeBool = false;
            _currentState   = null;
        }
        #endregion DISABLE AND RESET
    }
}

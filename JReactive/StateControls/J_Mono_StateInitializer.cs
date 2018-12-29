using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControls
{
    public class J_Mono_StateInitializer : MonoBehaviour
    {
        #region VALUES AND REFERENCES
        [BoxGroup("State Manager", true, true, 0)] [SerializeField, AssetsOnly, Required] private J_StateControl _stateControlToInitialize;
        //the main trakcer of the states
        [BoxGroup("State Manager", true, true, 0)] [SerializeField, AssetsOnly, Required] private J_StateTracker _statetracker;
        //a property to make sure the interaction is available
        [BoxGroup("State Manager", true, true, 0)] [ReadOnly] public bool InteractionAvailable { get { return _stateControlToInitialize.InteractionAvailable; } }

        //the initialization state of the game
        [BoxGroup("Main States", true, true, 5)] [SerializeField, AssetsOnly, Required] private J_State _notStartedState;
        //the initialization state of the game
        [BoxGroup("Main States", true, true, 5)] [SerializeField, AssetsOnly, Required] private J_State _firstState;
        #endregion

        #region INITIALIZATION
        //we launch at start so we can be sure that all items have passed the awake
        private void Start() { SceneInitialization(); }

        //here we set the order of items to call
        private void SceneInitialization()
        {
            //let the game start playing
            _stateControlToInitialize.Initialize(_notStartedState);
            _stateControlToInitialize.SetNewState(_firstState);
        }
        #endregion
    }
}
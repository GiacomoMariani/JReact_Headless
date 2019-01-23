using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControls
{
    public class J_Mono_StateInitializer : MonoBehaviour
    {
        #region VALUES AND REFERENCES
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_StateControl _stateControlToInitialize;
        //the main tracker of the states
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_StateTracker _statetracker;
        //the initialization state of the game
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State _notStartedState;
        //the initialization state of the game
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State _firstState;
        #endregion

        #region INITIALIZATION
        //we launch at start so we can be sure that all items have passed the awake
        private void Start() { SceneInitialization(); }

        //here we set the order of items to call
        private void SceneInitialization()
        {
            if(_statetracker) _statetracker.Initialize(_stateControlToInitialize);
            _stateControlToInitialize.Initialize(_notStartedState);
            _stateControlToInitialize.SetNewState(_firstState);
        }
        #endregion
    }
}
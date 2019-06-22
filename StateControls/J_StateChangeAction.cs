﻿using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace JReact.StateControl
{
    /// <summary>
    /// a simple action used to change the state
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/State Change Action")]
    public sealed class J_StateChangeAction : ScriptableObject
    {
        #region FIELDS AND PROPERTIES
        public UnityAction ThisAction => Process;
        [BoxGroup("State Control", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_SimpleStateControl _stateControl;
        [BoxGroup("State Control", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_State _desiredState;
        #endregion

        /// <summary>
        /// sets the desired state
        /// </summary>
        public void Process() { _stateControl.SetNewState(_desiredState); }
    }
}

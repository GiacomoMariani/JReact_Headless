using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace JReact.StateControl
{
    /// <summary>
    /// a simple action used to change the state
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/State Change Action")]
    public class J_StateChangeAction : ScriptableObject
    {
        #region FIELDS AND PROPERTIES
        public UnityAction ThisAction => ChangeState;
        [BoxGroup("State Control", true, true, 0), SerializeField, AssetsOnly, Required]
        protected J_SimpleStateControl _stateControl;
        [BoxGroup("State Control", true, true, 0), SerializeField, AssetsOnly, Required]
        protected J_State _desiredState;
        #endregion

        /// <summary>
        /// sets the desired state
        /// </summary>
        public void ChangeState() { _stateControl.SetNewState(_desiredState); }
    }
}

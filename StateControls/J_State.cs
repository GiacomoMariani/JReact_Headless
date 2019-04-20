using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl
{
    [CreateAssetMenu(menuName = "Reactive/Game States/Reactive State")]
    public class J_State : J_Service
    {
        // --------------- EVENTS --------------- //
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtStart = new JUnityEvent();
        [BoxGroup("Setup - Events", true, true, 5), SerializeField] private JUnityEvent _unityEvents_AtEnd = new JUnityEvent();

        public static T Copy<T>(T state)
            where T : J_State
        {
            var newState = CreateInstance<T>();
            newState.name                 = state.name + "_Copy";
            newState._unityEvents_AtStart = state._unityEvents_AtStart;
            newState._unityEvents_AtEnd   = state._unityEvents_AtEnd;
            return newState;
        }

        protected override void ActivateThis()
        {
            _unityEvents_AtStart?.Invoke();
            base.ActivateThis();
        }

        protected override void EndThis()
        {
            _unityEvents_AtEnd?.Invoke();
            base.EndThis();
        }
    }
}

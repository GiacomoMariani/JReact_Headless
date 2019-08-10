using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.StateControl
{
    /// <summary>
    /// this is used to track the weather
    /// </summary>
    public abstract class J_Mono_StateElement : MonoBehaviour
    {
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_State _state;

        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis() {}

        protected virtual void SanityChecks() { Assert.IsNotNull(_state, $"{gameObject.name} requires a _state"); }

        protected abstract void EnterState();

        protected abstract void ExitState();

        protected virtual void OnEnable()
        {
            _state.Subscribe(EnterState);
            _state.SubscribeToEnd(ExitState);
        }

        protected virtual void OnDisable()
        {
            _state.UnSubscribe(EnterState);
            _state.UnSubscribeToEnd(ExitState);
        }
    }
}

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public abstract class J_Mono_ServiceCallback : MonoBehaviour
    {
        //the states we want to call the event
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Service _service;

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        protected virtual void SanityChecks() => Assert.IsNotNull(_service, $"{gameObject.name} requires a {nameof(_service)}");

        protected virtual void InitThis()
        {
            _service.Subscribe(ServiceStart);
            _service.SubscribeToEnd(ServiceEnd);
        }

        // --------------- IMPLEMENTATION --------------- //
        protected abstract void ServiceStart();
        protected abstract void ServiceEnd();

        // --------------- DESTROY --------------- //
        protected virtual void OnDestroy()
        {
            _service.UnSubscribe(ServiceStart);
            _service.UnSubscribeToEnd(ServiceEnd);
        }
    }
}

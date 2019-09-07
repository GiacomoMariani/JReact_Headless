using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    public sealed class J_Mono_ServiceInitialization : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Service[] _services;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _resetBeforeActivation;

        private void Initialize()
        {
            SanityChecks();
            JLog.Log($"{name} initialize with {_services.Length} services", JLogTags.Service, this);

            //process all states
            for (int i = 0; i < _services.Length; i++)
            {
                J_Service service = _services[i];
                //reset will end the service if it was active
                if (_resetBeforeActivation) service.ResetThis();
                if (!_services[i].IsActive) service.Activate();
            }

            JLog.Log($"{name} init completed for {_services.Length} services", JLogTags.Collection, this);
        }

        private void DeInitialize()
        {
            SanityChecks();

            JLog.Log($"{name} de initialize with {_services.Length} services", JLogTags.Service, this);
            //process all states
            for (int i = 0; i < _services.Length; i++)
            {
                J_Service service = _services[i];
                service.End();
            }

            JLog.Log($"{name} de init completed for {_services.Length} services", JLogTags.Collection, this);
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_services, $"{name} requires a _orderedStates");
            Assert.IsTrue(_services.Length > 0, $"{name} has nothing to initialize");
        }

        // --------------- LISTENER SETUP --------------- //
        private void OnEnable()  => Initialize();
        private void OnDisable() => DeInitialize();

#if UNITY_EDITOR
        [BoxGroup("Conversion", true, true, 100), SerializeField, AssetsOnly] private J_ServiceInitialization _initialization;

        [BoxGroup("Conversion", true, true, 100), Button(ButtonSizes.Medium)]
        private void ImportFromScriptableObject() => _services = _initialization.GetServices();
#endif
    }
}

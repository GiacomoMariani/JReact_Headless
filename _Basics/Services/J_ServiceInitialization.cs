using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Initialization")]
    public class J_ServiceInitialization : ScriptableObject
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Service[] _services;
        //this can be used to force reactivation
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _resetBeforeActivation = false;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool? _initializing = null;

        public void Initialize()
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

        public void DeInitialize()
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

        public void ResetAll()
        {
            JLog.Log($"{name} resets for {_services.Length} services", JLogTags.Collection, this);
            for (int i = 0; i < _services.Length; i++)
                _services[i].ResetThis();

            JLog.Log($"{name} resets complete for {_services.Length} services", JLogTags.Collection, this);
        }
    }
}

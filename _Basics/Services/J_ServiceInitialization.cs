using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    [CreateAssetMenu(menuName = "Reactive/Basics/Initialization")]
    public class J_ServiceInitialization : J_Service
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Service[] _services;
        //this can be used to force reactivation
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _resetBeforeActivation = false;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool? _initializing = null;

        public void Initialize()
        {
            _initializing = true;
            Activate();
            _initializing = null;
        }

        public void DeInitialize()
        {
            _initializing = false;
            Activate();
            _initializing = null;
        }

        protected override void ActivateThis()
        {
            base.ActivateThis();

            if (!_initializing.HasValue)
            {
                JLog.Warning("Please use Initialize and DeInitialize, not Activate, for this service", JLogTags.Service, this);
                return;
            }

            JLog.Log(_initializing.Value
                         ? $"{name} initialize with {_services.Length} services"
                         : $"{name} de initialize with {_services.Length} services", JLogTags.Service, this);

            SanityChecks();

            //process all states
            for (int i = 0; i < _services.Length; i++)
            {
                J_Service service = _services[i];
                if (_initializing.Value)
                {
                    //reset will end the service if it was active
                    if (_resetBeforeActivation) service.ResetThis();
                    if (!_services[i].IsActive) service.Activate();
                }
                else
                    service.End();
            }

            End();
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_services, $"{name} requires a _orderedStates");
            Assert.IsTrue(_services.Length > 0, $"{name} has nothing to initialize");
        }

        protected override void EndThis()
        {
            base.EndThis();
            JLog.Log($"{name} task completed for {_services.Length} services", JLogTags.Collection, this);
        }

        public override void ResetThis()
        {
            base.ResetThis();
            JLog.Log($"{name} resets for {_services.Length} services", JLogTags.Collection, this);
            for (int i = 0; i < _services.Length; i++)
                _services[i].ResetThis();
        }
    }
}

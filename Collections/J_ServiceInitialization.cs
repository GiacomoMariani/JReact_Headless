using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    [CreateAssetMenu(menuName = "Reactive/Collection/Initialization")]
    public class J_ServiceInitialization : J_Service
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Service[] _services;
        
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _initializing = true;

        public static J_ServiceInitialization CreateInstance(J_Service[] statesToInitialize)
        {
            var ordered = CreateInstance<J_ServiceInitialization>();
            ordered._services = statesToInitialize;
            return ordered;
        }

        public void Initialize()
        {
            _initializing = true;
            Activate();
        }

        public void DeInitialize()
        {
            _initializing = false;
            Activate();
        }

        protected override void ActivateThis()
        {
            base.ActivateThis();
            if (_initializing)
                JLog.Log($"{name} initialize with {_services.Length} services", JLogTags.Collection, this);
            else
                JLog.Log($"{name} de initialize with {_services.Length} services", JLogTags.Collection, this);

            SanityChecks();

            //process all states
            for (int i = 0; i < _services.Length; i++)
            {
                if (_initializing) _services[i].Activate();
                else _services[i].End();
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

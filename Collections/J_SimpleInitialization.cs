using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    [CreateAssetMenu(menuName = "Reactive/Collection/Initialization")]
    public class J_SimpleInitialization : J_State
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Event[] _orderedEvents;

        public static J_SimpleInitialization CreateInstance(J_State[] statesToInitialize)
        {
            var ordered = CreateInstance<J_SimpleInitialization>();
            ordered._orderedEvents = statesToInitialize;
            return ordered;
        }

        public override void Activate()
        {
            base.Activate();
            JConsole.Log($"{name} initialize with {_orderedEvents.Length} events", JLogTags.Collection, this);
            SanityChecks();

            //process all states
            for (int i = 0; i < _orderedEvents.Length; i++)
                _orderedEvents[i].Activate();

            End();
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_orderedEvents, $"{name} requires a _orderedStates");
            Assert.IsTrue(_orderedEvents.Length > 0, $"{name} has nothing to initialize");
        }

        public override void End()
        {
            base.End();
            JConsole.Log($"{name} initialization complete for {_orderedEvents.Length} events", JLogTags.Collection, this);
        }
    }
}

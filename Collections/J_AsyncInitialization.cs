using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// used to set an order of initialization
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Collection/Async Initialization")]
    public class J_AsyncInitialization : J_Service
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Service[] _orderedStates;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TaskQueue _queue;

        public static J_AsyncInitialization CreateInstance(J_Service[] statesToInitialize)
        {
            var ordered = CreateInstance<J_AsyncInitialization>();
            ordered._orderedStates = statesToInitialize;
            ordered._queue = J_TaskQueue.CreateInstance();
            return ordered;
        }
        
        public override void Activate()
        {
            base.Activate();
            JConsole.Log($"{name} initialize for {_orderedStates.Length} states", JLogTags.Collection, this);
            SanityChecks();

            //process all states
            for (int i = 0; i < _orderedStates.Length; i++)
                _queue.ProcessTask(_orderedStates[i]);

            //confirm directly if all states are already processed or wait them
            if (!_queue.IsActive) End();
            else _queue.SubscribeToEnd(InitializationComplete);
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_queue, $"{name} requires a _queue");
            Assert.IsNotNull(_orderedStates, $"{name} requires a _orderedStates");
            Assert.IsTrue(_orderedStates.Length > 0, $"{name} has nothing to initialize");
        }

        //used to unsubscribe if the queue needs more time
        private void InitializationComplete()
        {
            _queue.UnSubscribeToEnd(InitializationComplete);
            End();
        }

        public override void End()
        {
            base.End();
            JConsole.Log($"{name} initialization complete for {_orderedStates.Length} states", JLogTags.Collection, this);
        }

        public override void ResetThis()
        {
            base.ResetThis();
            if (IsActive) _queue.UnSubscribeToEnd(InitializationComplete);
        }
    }
}

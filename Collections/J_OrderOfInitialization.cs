using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// used to set an order of initialization
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Collection/Ordered Initialization")]
    public class J_OrderOfInitialization : J_State
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State[] _orderedStates;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TaskQueue _queue;
        #endregion

        public static J_OrderOfInitialization CreateInstance(J_State[] statesToInitialize)
        {
            var ordered = CreateInstance<J_OrderOfInitialization>();
            ordered._orderedStates = statesToInitialize;
            ordered._queue = J_TaskQueue.CreateInstance();
            return ordered;
        }
        
        public override void Activate()
        {
            base.Activate();
            JConsole.Log($"{name} start initialization for {_orderedStates.Length} states", JLogTags.State, this);
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
            JConsole.Log($"{name} initialization complete", JLogTags.State, this);
        }

        public override void ResetThis()
        {
            base.ResetThis();
            if (IsActive) _queue.UnSubscribeToEnd(InitializationComplete);
        }
    }
}

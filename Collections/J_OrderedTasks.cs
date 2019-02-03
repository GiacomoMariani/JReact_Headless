using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// used to set an order of initialization
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Collection/Ordered Initialization")]
    public class J_OrderedTasks : J_State
    {
        #region FIELDS AND PROPERTIES
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_State[] _orderedStates;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TaskQueue _queue;
        #endregion

        public override void Activate()
        {
            base.Activate();
            JConsole.Log($"{name} start initialization for {_orderedStates.Length} states", JLogTags.State, this);
            //process all states
            for (int i = 0; i < _orderedStates.Length; i++)
                _queue.ProcessTask(_orderedStates[i]);

            //confirm directly if all states are already processed or wait them
            if (!_queue.IsActive) End();
            else _queue.SubscribeToEnd(InitializationComplete);
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

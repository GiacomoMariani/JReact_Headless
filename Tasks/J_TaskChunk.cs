using System.Collections.Generic;
using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// contains multiple tasks related to a single goal, such as a mission
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Task/Task Chunk")]
    public class J_TaskChunk : J_State
    {
        #region FIELDS AND PROPERTIES
        private JGenericDelegate<J_TaskChunk> OnComplete;
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Task[] _tasks;
        [InfoBox("Null => Cannot enqueue"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly]
        private J_TaskQueue _taskQueue;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Collection_DormantTasks _dormants;
        internal J_Collection_DormantTasks Dormants => _dormants;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] public ChunkState State { get; private set; } =
            ChunkState.NotStarted;
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] public string Title => name;
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] private List<J_Task> _activeTasks = new List<J_Task>();
        #endregion

        #region SANITY CHECKS
        private void SanityChecks()
        {
            Assert.IsNotNull(_tasks, $"({name}) requires a _tutorialSteps");
            Assert.IsTrue(_tasks.Length > 0, $"({name}) needs at least one item for _tutorialSteps");
            Assert.IsNotNull(Dormants, $"{name} requires a Dormants");
        }
        #endregion

        #region COMMANDS
        /// <summary>
        /// launch the tasks with a queue
        /// </summary>
        public void EnqueueTask()
        {
            Assert.IsNotNull(_taskQueue, $"{name} requires a _taskQueue");
            if (_taskQueue.Contains(this)) return;
            _taskQueue.ProcessTask(this);
        }

        /// <summary>
        /// launch this task chunk
        /// </summary>
        public override void Activate()
        {
            base.Activate();
            // --------------- START --------------- //
            JConsole.Log($"{name} CHUNK START--------------", JLogTags.Task, this);
            SanityChecks();
            State = ChunkState.Active;

            // --------------- INITIALIZE --------------- //
            for (int i = 0; i < _tasks.Length; i++)
            {
                _tasks[i].InjectChunk(this);
                _tasks[i].SubscribeToComplete(StepCompleted);
                _tasks[i].Activate();
                _activeTasks.Add(_tasks[i]);
            }

            //raise start event
            base.Activate();
        }

        //remove the task when completed
        private void StepCompleted(J_Task completedTask)
        {
            // --------------- REMOVAL --------------- //
            completedTask.UnSubscribeToComplete(StepCompleted);
            completedTask.End();
            _activeTasks.Remove(completedTask);

            // --------------- CHUNK COMPLETE --------------- //
            //the chunk completes when there are no more active tasks
            if (_activeTasks.Count > 0) return;
            JConsole.Log($"{name} CHUNK COMPLETE--------------", JLogTags.Task, this);
            State = ChunkState.Completed;
            OnComplete?.Invoke(this);
            End();
        }

        /// <summary>
        /// sets a specific state
        /// </summary>
        public void SetState(ChunkState stateLoaded) { State = stateLoaded; }
        #endregion

        #region SUBSCRIBERS
        public void SubscribeToComplete(JGenericDelegate<J_TaskChunk> actionToAdd) { OnComplete      += actionToAdd; }
        public void UnSubscribeToComplete(JGenericDelegate<J_TaskChunk> actionToRemove) { OnComplete -= actionToRemove; }
        #endregion

        #region DISABLE AND RESET
        protected virtual void OnDisable() { ResetThis(); }

        public override void ResetThis()
        {
            base.ResetThis();
            if (State == ChunkState.Completed) State = ChunkState.NotStarted;
            if (State != ChunkState.Active) return;
            foreach (var step in _activeTasks)
            {
                step.ResetThis();
                step.UnSubscribeToComplete(StepCompleted);
            }

            _activeTasks.Clear();
            State = ChunkState.NotStarted;
        }
        #endregion
    }

    public enum ChunkState { NotStarted = 0, Active = 100, Completed = 200 }
}

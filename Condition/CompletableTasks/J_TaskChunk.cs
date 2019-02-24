using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// contains multiple tasks related to a single goal, such as a mission
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Task/Task Chunk")]
    public class J_TaskChunk : J_Service, iObservable<J_TaskChunk>
    {
        #region FIELDS AND PROPERTIES
        private JGenericDelegate<J_TaskChunk> OnStateChange;
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_CompletableTask[] _tasks;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Collection_DormantTasks _dormants;

        // --------------- STATE --------------- //
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] public string Title => name;
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly]
        private List<J_CompletableTask> _activeTasks = new List<J_CompletableTask>();
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] private ChunkState _state =
            ChunkState.NotStarted;
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] public ChunkState State
        {
            get => _state;
            private set
            {
                _state = value;
                OnStateChange?.Invoke(this);
                JConsole.Log($"{name} enter state: {State}", JLogTags.Task, this);
            }
        }
        #endregion

        #region GENERATORS AND CHECKS
        public static J_TaskChunk CreateChunk(J_CompletableTask[] tasks, J_Collection_DormantTasks dormants, string nameSet = "Chunk")
        {
            var chunk = CreateInstance<J_TaskChunk>();
            chunk._tasks    = tasks;
            chunk._dormants = dormants;
            chunk.SanityChecks();
            chunk.name = nameSet;
            return chunk;
        }

        private void SanityChecks()
        {
            Assert.IsNotNull(_tasks, $"({name}) requires a _tutorialSteps");
            Assert.IsTrue(_tasks.Length > 0, $"({name}) needs at least one item for _tutorialSteps");
            Assert.IsNotNull(_dormants, $"{name} requires a _dormants");
        }
        #endregion

        #region COMMANDS
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
                var nextTask = _tasks[i];
                nextTask.SubscribeToTaskChange(StepCompleted);
                if (!nextTask.IsActive) nextTask.Activate();
                _activeTasks.Add(nextTask);
                if (_dormants != null) _dormants.TrackTask(nextTask);
            }
        }

        //remove the task when completed
        private void StepCompleted(J_CompletableTask completedTask)
        {
            if (completedTask.State != TaskState.Complete) return;
            // --------------- REMOVAL --------------- //
            completedTask.UnSubscribeToTaskChange(StepCompleted);
            completedTask.End();
            _activeTasks.Remove(completedTask);

            // --------------- CHUNK COMPLETE --------------- //
            //the chunk completes when there are no more active tasks
            if (_activeTasks.Count > 0) return;
            JConsole.Log($"{name} CHUNK COMPLETE--------------", JLogTags.Task, this);
            State = ChunkState.Completed;
            End();
        }

        /// <summary>
        /// sets a specific state
        /// </summary>
        public void SetState(ChunkState stateLoaded) { State = stateLoaded; }
        #endregion

        #region SUBSCRIBERS
        public void SubscribeToWindChange(JGenericDelegate<J_TaskChunk> action) { OnStateChange   += action; }
        public void UnSubscribeToWindChange(JGenericDelegate<J_TaskChunk> action) { OnStateChange -= action; }
        public void SubscribeToStateChange(JGenericDelegate<J_TaskChunk> action) { SubscribeToWindChange(action); }
        public void UnSubscribeToStateChange(JGenericDelegate<J_TaskChunk> action) { UnSubscribeToWindChange(action); }
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
                step.UnSubscribeToTaskChange(StepCompleted);
            }

            _activeTasks.Clear();
            State = ChunkState.NotStarted;
        }
        #endregion
    }

    public enum ChunkState { NotStarted = 0, Active = 100, Completed = 200 }
}

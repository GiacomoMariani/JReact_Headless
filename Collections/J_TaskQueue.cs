using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Collections
{
    /// <summary>
    /// handles tasks in a given order
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Collection/Task Queue")]
    public class J_TaskQueue : J_Service, IEnumerable<iTask>
    {
        #region FIELDS AND PROPERTIS
        public const string NoTask = "QueueFree_NoTask";
        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iTask _currentTask;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Queue<iTask> _taskQueue = new Queue<iTask>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int TotalTasks => _taskQueue.Count;

        [FoldoutGroup("Book Keeping", false, 10), ReadOnly, ShowInInspector] public string CurrentTaskName => _currentTask != null
                                                                                                                  ? _currentTask.Name
                                                                                                                  : NoTask;
        #endregion

        #region COMMANDS
        public static J_TaskQueue CreateInstance() { return CreateInstance<J_TaskQueue>(); }

        public void ProcessTask(iTask taskToProcess)
        {
            JConsole.Log($"{name} task added. Current tasks: {TotalTasks}", JLogTags.Task, this);
            //send this task if no task is running, otherwise enqueue it
            if (!IsActive) StartQueueWith(taskToProcess);
            else EnqueueTask(taskToProcess);
        }
        #endregion

        #region START AND END QUEUE
        //the first task activate the queue        
        private void StartQueueWith(iTask taskToProcess)
        {
            Activate();
            JConsole.Log($"{name} Task Queue START with {taskToProcess.Name}", JLogTags.Task, this);
            RunTask(taskToProcess);
        }

        private void StopQueue(iTask currentTask)
        {
            JConsole.Log($"{name} Task Queue STOP with {_currentTask.Name}", JLogTags.Task, this);
            End();
        }
        #endregion

        #region PROCESSING TASKS
        private void RunTask(iTask taskToProcess)
        {
            //send the task and wait for the next
            JConsole.Log($"{name} running task {taskToProcess.Name}", JLogTags.Task, this);
            _currentTask = taskToProcess;
            taskToProcess.SubscribeToEnd(CheckNext);
            taskToProcess.Activate();
        }

        private void EnqueueTask(iTask taskToProcess)
        {
            JConsole.Log($"{name} enqueued {taskToProcess.Name}", JLogTags.Task, this);
            _taskQueue.Enqueue(taskToProcess);
        }

        //this is called when the processed task is completed
        private void CheckNext()
        {
            StopTrackingTask();
            if (TotalTasks > 0) RunTask(_taskQueue.Dequeue());
        }
        #endregion

        #region DISABLE AND RESET
        public override void ResetThis()
        {
            base.ResetThis();
            if (_currentTask != null) StopTrackingTask();
            _taskQueue.Clear();
        }

        private void StopTrackingTask()
        {
            _currentTask.UnSubscribeToEnd(CheckNext);
            if (_taskQueue.Count == 0) StopQueue(_currentTask);
            _currentTask = null;
        }
        #endregion

        #region QUEUE CLASS
        public IEnumerator<iTask> GetEnumerator() { return _taskQueue.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public int Count => _taskQueue.Count;
        public bool Contains(iTask state) { return _taskQueue.Contains(state); }
        #endregion
    }
}

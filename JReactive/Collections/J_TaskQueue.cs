using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Collections
{
    /// <summary>
    /// handles tasks in a given order
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Collection/Task Queue")]
    public class J_TaskQueue : ScriptableObject
    {
        #region FIELDS AND PROPERTIS
        // --------------- SETUP --------------- //
        //the max desired task
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _allocatedTasks = 10;

        // --------------- STATE --------------- //
        //the current processer using to process a task
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iTask _currentProcedure;
        //the queue of processors
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Queue<iTask> _procedureQueue = new Queue<iTask>();
        //to check if any task is running
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsActive
        {
            get { return _currentProcedure != null; }
        }
        //the total task procedures
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int TotalProcessors
        {
            get { return _procedureQueue.Count; }
        }
        #endregion

        #region COMMANDS
        /// <summary>
        /// used to process a task
        /// </summary>
        /// <param name="taskToProcess">the task to process</param>
        public void ProcessTask(iTask taskToProcess)
        {
            JConsole.Log($"{name} task added. Current tasks: {TotalProcessors}", J_LogConstants.Task, this);

            if (TotalProcessors >= _allocatedTasks)
            {
                JConsole.Warning($"{name} has too many tasks. Current {TotalProcessors} / Max {_allocatedTasks}.\nAborting Task: {taskToProcess.TaskName}",
                                 J_LogConstants.Task, this);
                return;
            }

            //send this task if no task is running, otherwise enqueue it
            if (!IsActive) RunTask(taskToProcess);
            else EnqueueTask(taskToProcess);
        }

        private void RunTask(iTask taskToProcess)
        {
            //send the task and wait for the next
            _currentProcedure = taskToProcess;
            taskToProcess.ThisTask.Invoke();
            taskToProcess.OnComplete += CheckNext;
        }

        private void EnqueueTask(iTask taskToProcess) { _procedureQueue.Enqueue(taskToProcess); }

        //this is called when te processed task is completed, and check if there's anything else in the queue
        private void CheckNext()
        {
            StopTrackingTask();
            if (TotalProcessors > 0) RunTask(_procedureQueue.Dequeue());
        }
        #endregion

        #region DISABLE AND RESET
        //we reset on disable, when the scriptable object goes out of scope
        private void OnDisable() { ResetThis(); }

        //used to reset the mouse
        public void ResetThis()
        {
            if (_currentProcedure != null) StopTrackingTask();
            _procedureQueue.Clear();
        }

        //stop tracking the current task
        private void StopTrackingTask()
        {
            _currentProcedure.OnComplete -= CheckNext;
            _currentProcedure            =  null;
        }
        #endregion
    }
}

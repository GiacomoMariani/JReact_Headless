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
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] private iTask _currentProcedure;
        //the queue of processors
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] private Queue<iTask> _procedureQueue = new Queue<iTask>();
        //to check if any task is running
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] public bool IsActive { get { return _currentProcedure != null; } }
        //the total task procedures
        [BoxGroup("State", true, true, 5), ShowInInspector, ReadOnly] private int TotalProcessors { get { return _procedureQueue.Count; } }
        #endregion

        #region COMMANDS
        /// <summary>
        /// used to process a task
        /// </summary>
        /// <param name="taskToProcess">the task to process</param>
        public void ProcessTask(iTask taskToProcess)
        {
            HelperConsole.DisplayMessage(string.Format("{0} task added. Current tasks: {1}", name, TotalProcessors));

            if (TotalProcessors >= _allocatedTasks)
            {
                HelperConsole.DisplayWarning(string.Format("{0} has too many tasks. Current {1} / Max {2}.\nAborting Task: {3}"
                                                           , name, TotalProcessors, _allocatedTasks, taskToProcess.TaskName));
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

using System;
using JReact.Collections;
using UnityEngine;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// stores the dormant tasks and reactivate them when required
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Task/Dormant Task")]
    public class J_Collection_DormantTasks : J_ReactiveCollection<J_CompletableTask>
    {
        internal void TrackTask(J_CompletableTask task) { task.SubscribeToTaskChange(CheckTask); }

        private void CheckTask(J_CompletableTask task)
        {
            switch (task.State)
            {
                case TaskState.WaitStartCondition: break;
                case TaskState.Active:             break;

                //add this to dormant at the correct state
                case TaskState.Dormant:
                    if (!Contains(task)) Add(task);
                    break;

                //remove from dormants when the task is completed or get back active)
                case TaskState.NotInitialized:
                case TaskState.Complete:
                    task.UnSubscribeToTaskChange(CheckTask);
                    if (Contains(task)) Remove(task);
                    break;

                case TaskState.ActivationWaiting:
                    if (Contains(task)) Remove(task);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        /// reactivates the tasks
        /// </summary>
        public void ReactivateAll()
        {
            for (int i = 0; i < Count; i++)
                _thisCollection[i].ReactivateTask();

            Clear();
        }
    }
}

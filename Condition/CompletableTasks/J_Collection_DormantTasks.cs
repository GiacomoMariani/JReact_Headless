using System;
using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// stores the dormant tasks and reactivate them when required
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Task/Dormant Task")]
    public class J_Collection_DormantTasks : J_ReactiveList<J_CompletableTask>
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool HasDormants => _ThisList.Count > 0;

        internal void TrackTask(J_CompletableTask task) { task.SubscribeToTaskChange(CheckTask); }

        private void CheckTask(J_CompletableTask task)
        {
            switch (task.State)
            {
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

                case TaskState.Startup:
                case TaskState.WaitStartCondition:
                case TaskState.Active:
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
            if (!HasDormants) return;

            for (int i = 0; i < Count; i++) _ThisList[i].ReactivateTask();

            Clear();
        }
    }
}

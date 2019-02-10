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
        /// <summary>
        /// reactivates the steps
        /// </summary>
        public void Reactivate()
        {
            for (int i = 0; i < Count; i++)
                _thisCollection[i].ReactivateTask();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Conditions.Tasks
{
    /// <summary>
    /// fully rest all the tasks
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Task/Tutorial Reset")]
    public class J_Task_Resetter : ScriptableObject, iResettable
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_TaskChunk[] _allChunks;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_Collection_DormantTasks[] _alldDormantCollections;


        public void ResetThis()
        {
            for (int i = 0; i < _allChunks.Length; i++)
                if (_allChunks[i].State == ChunkState.Active)
                    _allChunks[i].ResetThis();

            for (int i = 0; i < _alldDormantCollections.Length; i++)
                _alldDormantCollections[i].ResetThis();
        }
    }
}

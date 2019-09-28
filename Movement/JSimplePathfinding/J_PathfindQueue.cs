using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Pathfinding
{
    /// <summary>
    /// manages the requests of pathfinding
    /// </summary>
    public abstract class J_PathfindQueue<T> : ScriptableObject
        where T : J_PathNode
    {
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private J_AStar<T> _algorithm;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Queue<PathRequest> _pathQueue = new Queue<PathRequest>();
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private PathRequest _current;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isProcessing;

        // --------------- PATHFIND METHODS --------------- //
        /// <summary>
        /// request a search for a given path
        /// </summary>
        /// <param name="start">start node</param>
        /// <param name="goal">node to reach</param>
        /// <param name="getCost">function to calculate distance costs</param>
        /// <param name="isAccessible">function to calculate accessibility</param>
        /// <param name="actionToApply">the callback at the end of the path</param>
        public void FindPath(T               start, T goal, Func<T, T, float> getCost, Func<T, bool> isAccessible,
                             Action<List<T>> actionToApply)
        {
            _pathQueue.Enqueue(new PathRequest(start, goal, getCost, isAccessible, actionToApply));
            TryFindNext();
        }

        //start or enqueue the request
        private void TryFindNext()
        {
            //stop if still processing or if we have no further elements to search
            if (_isProcessing || _pathQueue.Count <= 0) return;
            //start processing
            _isProcessing = true;
            _current      = _pathQueue.Dequeue();
            _algorithm.CalculatePath(_current.start, _current.goal, _current.getCost, _current.isAccessible, CompleteRequest);
        }

        //sends when the request is complete
        private void CompleteRequest(List<T> path)
        {
            _isProcessing = false;
            _current.callback(path);
            TryFindNext();
        }

        // --------------- PATH REQUEST --------------- //
        //a request for the path
        [Serializable]
        private struct PathRequest
        {
            public T start;
            public T goal;
            public Func<T, T, float> getCost;
            public Func<T, bool> isAccessible;
            public Action<List<T>> callback;

            public PathRequest(T start, T goal, Func<T, T, float> getCost, Func<T, bool> isAccessible, Action<List<T>> callback)
            {
                this.start        = start;
                this.goal         = goal;
                this.getCost      = getCost;
                this.isAccessible = isAccessible;
                this.callback     = callback;
            }
        }

        // --------------- DISABLE AND RESET --------------- //
        protected virtual void OnDisable() { ResetThis(); }

        private void ResetThis()
        {
            _pathQueue.Clear();
            _isProcessing = false;
        }
    }
}

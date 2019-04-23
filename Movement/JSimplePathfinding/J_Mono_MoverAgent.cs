﻿using System;
using System.Collections.Generic;
using JReact.Movement;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pathfinding
{
    /// <summary>
    /// this class controls and agent that follows a path from start to end
    /// </summary>
    /// <typeparam name="T">the type of node we want to apply</typeparam>
    [RequireComponent(typeof(J_TransformMover))]
    public abstract class J_Mono_MoverAgent<T> : MonoBehaviour
        where T : J_PathNode
    {
        #region VALUES AND PROPERTIES
        // --------------- EVENTS --------------- //
        //path actions
        private event Action<T> OnStart;
        private event Action<T> OnComplete;

        //step action
        private event Action<(Vector2Int startStep, Vector2Int endStep)> OnStep;

        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, Range(0.1f, 1.5f)] private float _stepInSeconds = 0.75f;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_PathfindQueue<T> _pathQueue;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool IsMoving { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _doingStep;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public T CurrentNode { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected virtual Vector2 _CurrentPosition
        {
            get => transform.position;
            set => transform.position = value;
        }

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_TransformMover _mover;
        private J_TransformMover Mover
        {
            get
            {
                if (_mover == null) _mover = GetComponent<J_TransformMover>();
                return _mover;
            }
        }

        // --------------- DEBUG --------------- //
        //debug elements
        [BoxGroup("Debug", true, true, 100), SerializeField] private bool _debug;
        #endregion

        #region MOVE COMMANDS
        /// <summary>
        /// places the agent on a node
        /// </summary>
        /// <param name="node">place the agent on this node</param>
        public void PlaceAgentOn(T node)
        {
            CurrentNode      = node;
            _CurrentPosition = node.transform.position;
        }

        /// <summary>
        /// moves the agent on a given node
        /// </summary>
        /// <param name="goalNode">the node to reach</param>
        public void MoveTo(T goalNode)
        {
            Assert.IsNotNull(CurrentNode, $"{gameObject.name} requires a current node to start moving.");
            MoveThisAgent(CurrentNode, goalNode);
        }

        /// <summary>
        /// handles the specific movement on the path
        /// </summary>
        /// <param name="start">the node to start</param>
        /// <param name="goal">the node to reach</param>
        private void MoveThisAgent(T start, T goal)
        {
            if (_debug)
                JLog.Log($"{gameObject.name} moving from {start.Coordinates} to {goal.Coordinates}",
                         JLogTags.Pathfind, this);

            _pathQueue.FindPath(start, goal, J_PathCost.CalculateNodeDistance, CanAccessNode, StartMovement);
        }

        private void StartMovement(List<T> path)
        {
            Timing.RunCoroutine(MovingOnPath(path).CancelWith(gameObject), Segment.FixedUpdate, JCoroutineTags.COROUTINE_MoverAgent);
        }
        #endregion

        #region ABSTRACT IMPLEMENTATION
        /// <summary>
        /// to calculate node accessibility
        /// </summary>
        /// <param name="node">the data of the node to check if mover can access</param>
        /// <returns>true if the node is accessible</returns>
        protected abstract bool CanAccessNode(T node);
        #endregion

        #region MOVEMENT
        //moves the agent forward
        private IEnumerator<float> MovingOnPath(List<T> path)
        {
            // --------------- START MOVING --------------- //
            OnStart?.Invoke(CurrentNode);
            IsMoving = true;

            // --------------- LOOP --------------- //
            if (path != null)
                for (int i = 0; i < path.Count - 1; i++)
                {
                    // --------------- STEP START --------------- //
                    _doingStep = true;
                    OnStep?.Invoke((path[i].Coordinates, path[i + 1].Coordinates));

                    // --------------- MOVER ACTION --------------- //
                    Mover.SubscribeToReachFixedPosition(StepComplete);
                    Mover.MoveTransformToPosition(path[i + 1].transform.position, _stepInSeconds);

                    // --------------- UPDATE THE NODE --------------- //
                    CurrentNode = path[i + 1];
                    yield return Timing.WaitUntilFalse(GetDoingAStep);
                }

            // --------------- STOP MOVING --------------- //
            IsMoving = false;
            OnComplete?.Invoke(CurrentNode);
        }

        //sets the step as completed
        private void StepComplete(Vector3 newPosition) { _doingStep = false; }
        #endregion

        #region HELPER
        private bool GetDoingAStep() => _doingStep;
        #endregion

        #region SUBSCRIBERS
        public void SubscribeToPathStart(Action<T> actionToAdd) { OnStart                                    += actionToAdd; }
        public void UnSubscribeToPathStart(Action<T> actionToRemove) { OnStart                               -= actionToRemove; }

        public void SubscribeToPathComplete(Action<T> actionToAdd) { OnComplete                              += actionToAdd; }
        public void UnSubscribeToPathComplete(Action<T> actionToRemove) { OnComplete                         -= actionToRemove; }

        public void SubscribeToStep(Action<(Vector2Int startStep, Vector2Int endStep)> actionToAdd) { OnStep += actionToAdd; }
        public void UnSubscribeToStep(Action<(Vector2Int startStep, Vector2Int endStep)> actionToRemove) { OnStep -= actionToRemove; }
        #endregion
    }
}

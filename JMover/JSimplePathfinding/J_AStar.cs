using Priority_Queue;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pathfinding
{
    /// <summary>
    /// the algorithm to calculate the pathfinding
    /// </summary>
    public abstract class J_AStar<T> : ScriptableObject
        where T : J_PathNode
    {
        #region VALUES AND PROPERTIES
        // --------------- SETUP --------------- //
        //the related grid
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_PathGrid<T> _pathGrid;
        //heuristic, used to calculate the path
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_PathHeuristics _heuristics;
        //max amount of steps for a path
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _maxPathSteps = 1000;

        // --------------- STATE --------------- //
        //estimated distances
        private Dictionary<T, float> _heuristicCost = new Dictionary<T, float>();
        //real distances
        private Dictionary<T, float> _realCost = new Dictionary<T, float>();
        //node parents
        private Dictionary<T, T> _parentNodes = new Dictionary<T, T>();
        //already explored
        private HashSet<T> _exploredNodes = new HashSet<T>();
        //still to explore
        private SimplePriorityQueue<T, float> _openList = new SimplePriorityQueue<T, float>();
        //the result path
        private List<T> _resultPath = new List<T>();

        // --------------- DEBUG --------------- //
        [BoxGroup("Debug", true, true, 100), SerializeField] private bool _debug = false;
        #endregion

        #region PATHFINDING ALGORITHM
        /// <summary>
        /// calculates the path from start to goal
        /// </summary>
        /// <param name="start">start node</param>
        /// <param name="goal">goal node</param>
        /// <param name="getCost">used to calculate costs, such as J_PatchCost.CalculateNodeDistance</param>
        /// <param name="isAccessible">to check if a node is accessible</param>
        /// <param name="callback">the action set at the end of the calculation</param>
        public void CalculatePath(T start, T goal, Func<T,T,float> getCost, Func<T, bool> isAccessible, Action<List<T>> callback)
        {
            var path = Internal_CalculatePath(start, goal, getCost, isAccessible);
            callback(path);
        }

        //calculates the path
        private List<T> Internal_CalculatePath(T startNode, T goalNode, Func<T,T,float> getCost, Func<T, bool> isAccessible)
        {
            if (_debug)
                JConsole.Log($"{name} calculates from {startNode.Coordinates} to {goalNode.Coordinates}",
                             J_LogTags.Pathfind, this);

            // --------------- SETTING UP THE CALCULATION --------------- //
            var goalReached = false;
            ResetCollections();
            //starting distance is 0
            _realCost[startNode] = 0;
            //setting the heuristic cost of the start node
            _heuristicCost[startNode] = _heuristics.EstimateCost(startNode.Coordinates, goalNode.Coordinates);
            //initially, only the start node is known.
            _openList.Enqueue(startNode, _heuristicCost[startNode]);

            // --------------- ITERATION OF THE NEIGHBOURS --------------- //
            //iterates until _openList is not empty
            while (_openList.Count > 0)
            {
                //get the first node on the list
                var currentNode = _openList.Dequeue();
                //if this is the goal we reached the end of the path
                if (currentNode == goalNode)
                {
                    goalReached = true;
                    break;
                }

                //otherwise we add it to the explored list
                _exploredNodes.Add(currentNode);

                //the check all the neighbours
                var neighbours = _pathGrid.GetNeighboursOf(currentNode);
                for (var i = 0; i < neighbours.Count; i++)
                {
                    var neighbour = neighbours[i];
                    // --------------- ACCESSIBILITY --------------- //
                    //ignore inaccessible
                    if (!isAccessible(neighbour)) continue;

                    // --------------- SCORE CALCULATION --------------- //
                    //get the node cost from the current path 
                    var tentativeScore = _realCost[currentNode] + getCost(currentNode, neighbour);
                    //ignore if we have already a better path until here
                    if (_realCost.ContainsKey(neighbour) &&
                        tentativeScore >= _realCost[neighbour]) { continue; }

                    // --------------- REPLACEMENT --------------- //
                    //set the new cost
                    _realCost[neighbour] = tentativeScore;
                    //recalculate the heuristic cost
                    _heuristicCost[neighbour] = _realCost[neighbour] +
                                                _heuristics.EstimateCost(neighbour.Coordinates,
                                                                                  goalNode.Coordinates);
                    //set the parent node
                    _parentNodes[neighbour] = currentNode;

                    // --------------- ADDING TO OPEN SET --------------- //
                    //add the neighbour to the list if missing
                    if (!_openList.Contains(neighbour))
                        _openList.Enqueue(neighbour, _heuristicCost[neighbour]);
                }
            }

            if (_debug)
                JConsole.Log($"{name} Path from {startNode.Coordinates} to {goalNode.Coordinates}. Found: {goalReached}. Explored {_exploredNodes.Count} nodes.",
                             J_LogTags.Input, this);
            if (_debug && !goalReached)
                JConsole.Log($"{name} No path Found. Nodes Explored {_exploredNodes.PrintAll()}", J_LogTags.Input, this);

            //if we have not reached the goal we return null
            if (!goalReached ||
                goalNode == startNode)
                return null;

            //calculates the path backward
            var path = RecalculatePath(goalNode, startNode);

            return path;
        }
        #endregion

        #region HELPERS
        private void ResetCollections()
        {
            _heuristicCost.Clear();
            _realCost.Clear();
            _parentNodes.Clear();
            _openList.Clear();
            _exploredNodes.Clear();
            _resultPath.Clear();
        }

        //calculates the path backwards into an array
        private List<T> RecalculatePath(T goal, T start)
        {
            //to count the step (used as safecheck for circular paths)
            int steps = 0;

            //ignore null goals
            Assert.IsNotNull(goal, $"{name} is calculating a path with null goal.  Path {_resultPath.PrintAll()}");

            //run until we reach the goal
            while (goal != start)
            {
                //count and move to the next node
                steps++;
                _resultPath.Add(goal);
                goal = _parentNodes[goal];

                //check for too long paths (circular path issue)
                if (steps > _maxPathSteps)
                {
                    JConsole.Warning($"{name}Path too long. Path {_resultPath.PrintAll()}", J_LogTags.Pathfind, this);
                    break;
                }
            }

            //add start node and reverse it
            _resultPath.Add(start);
            _resultPath.Reverse();

            //log and return
            if (_debug) JConsole.Log($"{name} Path found. Nodes: {_resultPath.Count}.", J_LogTags.Pathfind, this);
            return _resultPath;
        }
        #endregion
    }
}

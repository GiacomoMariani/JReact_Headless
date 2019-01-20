﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JReact.Pathfinding
{
    /// <summary>
    /// this the heuristic to calculate the A* pathfinding h cost (heuristic cost)
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Pathfinding/Heuristics")]
    public class JPathHeuristics : ScriptableObject
    {
        #region VALUES AND PROPERTIES
        public enum HeuristicsType { Euclidean, Manhattan }
        public HeuristicsType _desiredHeuristic;
        #endregion

        /// <summary>
        /// calculates the heuristic cost of 2 nodes
        /// </summary>
        /// <param name="start">start node</param>
        /// <param name="goal">goal node</param>
        /// <returns>returns the estimated cost</returns>
        public float EstimateCost(Vector2 start, Vector2 goal)
        {
            switch (_desiredHeuristic)
            {
                case HeuristicsType.Euclidean:
                    return EuclideanEstimate(start, goal);
                case HeuristicsType.Manhattan:
                    return ManhattanEstimate(start, goal);
                default:
                    throw new ArgumentOutOfRangeException("_desiredHeuristic", _desiredHeuristic, "Heuristics not found");
            }
        }

        /// <summary>
        /// the euclidead estimate
        /// </summary>
        private float EuclideanEstimate(Vector2 startPosition, Vector2 goalPosition)
        {
            return Mathf.Sqrt(Mathf.Pow(startPosition.x - goalPosition.x, 2) +
                              Mathf.Pow(startPosition.y - goalPosition.y, 2));
        }

        /// <summary>
        /// the manhattan estimate
        /// </summary>
        private float ManhattanEstimate(Vector2 startPosition, Vector2 goalPosition)
        {
            return (Mathf.Abs(startPosition.x - goalPosition.x) +
                    Mathf.Abs(startPosition.y - goalPosition.y));
        }
    }
}

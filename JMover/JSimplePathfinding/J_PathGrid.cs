using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.Pathfinding
{
    /// <summary>
    /// the grid for the pathfinding
    /// </summary>
    public abstract class J_PathGrid<T> : ScriptableObject
        where T : J_PathNode
    {
        #region FIELDS AND PROPERTIES
        //the nodes related to this, each node, connected with a neighbour
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<T, List<T>> _nodeGrid = new Dictionary<T, List<T>>();
        #endregion

        //used to connect to nodes
        internal void ConnectNode(T node, T nodeConnected)
        {
            // --------------- STEP 1 ADD THE KEY IF REQUIRED --------------- //
            if (!_nodeGrid.ContainsKey(node))
                _nodeGrid[node] = new List<T>() { nodeConnected };

            // --------------- STEP 2 ADD IF NOT ALREADY NEIGHBOUR--------------- //
            else if (!_nodeGrid[node].Contains(nodeConnected))
                _nodeGrid[node].Add(nodeConnected);
        }

        //used to get the neighbours of a given node
        internal List<T> GetNeighboursOf(T node) { return _nodeGrid[node]; }
    }
}

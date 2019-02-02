namespace JReact.Pathfinding
{
    /// <summary>
    /// used to calculate cost between 2 nodes
    /// </summary>
    public class J_PathCost
    {
        // --------------- WEIGHT CONSTANTS --------------- //
        //the price to multiply for linear connections
        public const float PATHWEIGHT_Linear = 1f;
        //the price to multiply for diagonal connections
        public const float PATHWEIGHT_Diagonal = 1.414f;

        /// <summary>
        /// calculates the distance between two nodes
        /// </summary>
        /// <param name="nodeA">start node</param>
        /// <param name="nodeB">goal node</param>
        /// <returns>calculates the distance between 2 nodes</returns>
        public static float CalculateNodeDistance(J_PathNode nodeA, J_PathNode nodeB)
        {
            //get the basic cost from the node weights
            float cost = nodeA.baseWeight + nodeB.baseWeight;
            //add multiplier for diagonals
            //if the xs or the ys are not the same we've a diagonal, so we add the diagonal cost
            if (!(nodeA.Coordinates.x == nodeB.Coordinates.x ||
                  nodeA.Coordinates.y == nodeB.Coordinates.y))
                cost *= PATHWEIGHT_Diagonal;
            return cost;
        }
    }
}

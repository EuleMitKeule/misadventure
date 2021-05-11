using UnityEngine;
using System;

namespace HotlineHyrule.Pathfinding
{
    public class NavNode
    {
        /// <summary>
        /// The cell position associated with this node.
        /// </summary>
        public readonly Vector3Int Position;
        /// <summary>
        /// The node located before this node in the final path.
        /// </summary>
        public NavNode Parent;
        /// <summary>
        /// Distance to start node.
        /// </summary>
        public int CostG;
        /// <summary>
        /// Distance to end node.
        /// </summary>
        public int CostH;
        /// <summary>
        /// Total distance cost.
        /// </summary>
        public int CostF => CostG + CostH;

        public NavNode(Vector3Int position)
        {
            Position = position;
        }
    }
}
using UnityEngine;
using System;

namespace HotlineHyrule.Pathfinding
{
    public class NavNode
    {
        public readonly Vector3Int Position;
        int hashCode;
        public NavNode Parent;
        public int CostG;
        public int CostH;
        public int CostF => CostG + CostH;

        public NavNode(Vector3Int position)
        {
            Position = position;
        }
    }
}
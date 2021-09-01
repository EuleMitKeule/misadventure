using System;
using UnityEngine;

namespace HotlineHyrule.Pathfinding
{
    public class CellEventArgs : EventArgs
    {
        /// <summary>
        /// The position in cell space.
        /// </summary>
        public Vector3Int CellPosition { get; }

        public CellEventArgs(Vector3Int cellPosition)
        {
            CellPosition = cellPosition;
        }
    }
}
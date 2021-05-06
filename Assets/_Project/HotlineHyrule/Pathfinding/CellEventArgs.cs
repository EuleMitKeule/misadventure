using System;
using UnityEngine;

namespace HotlineHyrule.Pathfinding
{
    public class CellEventArgs : EventArgs
    {
        public Vector3Int CellPosition { get; set; }

        public CellEventArgs(Vector3Int cellPosition)
        {
            CellPosition = cellPosition;
        }
    }
}
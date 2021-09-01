using UnityEngine;

namespace Misadventure.Extensions
{
    public static class Vector2IntExtensions
    {
        /// <summary>
        /// Converts a cell position to the cell's center point in world space 
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <returns></returns>
        public static Vector2 ToWorld(this Vector2Int cellPosition) => cellPosition + Vector2.one * 0.5f;
    }
}
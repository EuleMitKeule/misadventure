using UnityEngine;

namespace Misadventure.Extensions
{
    public static class Vector3IntExtensions
    {
        /// <summary>
        /// Converts a cell position to the cell's center point in world space 
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <returns></returns>
        public static Vector3 ToWorld(this Vector3Int cellPosition) => cellPosition + (Vector3)Vector2.one * 0.5f;
    }
}
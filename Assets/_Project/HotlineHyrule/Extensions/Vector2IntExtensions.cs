using UnityEngine;

namespace HotlineHyrule.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Vector2 ToWorld(this Vector2Int cellPosition) => cellPosition + Vector2.one * 0.5f;
    }
}
using System.Linq;
using UnityEngine;

namespace Misadventure.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Rotates the vector by the given angle.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            var x = Mathf.Cos(Mathf.Deg2Rad * angle) * vector.x - Mathf.Sin(Mathf.Deg2Rad * angle) * vector.y;
            var y = Mathf.Sin(Mathf.Deg2Rad * angle) * vector.x + Mathf.Cos(Mathf.Deg2Rad * angle) * vector.y;
            return new Vector2(x, y);
        }

        public static bool ContainsPoint(this Vector2[] polygon, Vector2 point)
        {
            var j = polygon.Length - 1;
            var inside = false;
            for (var i = 0; i < polygon.Length; j = i++)
            {
                var pi = polygon[i];
                var pj = polygon[j];
                if (((pi.y <= point.y && point.y < pj.y) || (pj.y <= point.y && point.y < pi.y)) &&
                    (point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x))
                    inside = !inside;
            }

            return inside;
        }

        public static Vector2 To(this Vector2 from, Vector2 to) => to - from;

        public static Vector2 DirectionTo(this Vector2 from, Vector2 to) => (to - from).normalized;
        public static float DistanceTo(this Vector2 from, Vector2 to) => (to - from).magnitude;

        public static bool ContainsPolygon(this Vector2[] polygon, Vector2[] otherPolygon) =>
            otherPolygon.All(polygon.ContainsPoint);
    }
}
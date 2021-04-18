using UnityEngine;

namespace HotlineHyrule.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Rotates the vector by the given angle.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateAroundZ(this Vector3 vector, float angle)
        {
            var x = Mathf.Cos(Mathf.Deg2Rad * angle) * vector.x - Mathf.Sin(Mathf.Deg2Rad * angle) * vector.y;
            var y = Mathf.Sin(Mathf.Deg2Rad * angle) * vector.x + Mathf.Cos(Mathf.Deg2Rad * angle) * vector.y;
            return new Vector3(x, y, vector.z);
        }
    }
}
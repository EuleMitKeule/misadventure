using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace HotlineHyrule.Extensions
{
    public static class ComplexExtensions
    {
        public static Vector2 ToVector2(this Complex c) => new Vector2((float)c.Real, (float)c.Imaginary);
        
        public static Vector3 ToVector3(this Complex c) => new Vector3((float)c.Real, (float)c.Imaginary, 0f);
    }
}
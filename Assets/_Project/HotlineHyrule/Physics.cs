using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace HotlineHyrule
{
    public static class Physics
    {
        public static Collider2D[] OverlapConeAll(Vector2 center, float radius, Vector2 direction, float leftAngle, float rightAngle, float rotation, int layerMask)
        {
            var colliders = Physics2D.OverlapCircleAll(center, radius, layerMask);
            var collidersInCone = new List<Collider2D>();

            foreach (var collider in colliders)
            {
                var colliderAngle = Vector2.SignedAngle(direction, collider.bounds.center);
                
                if (leftAngle + rotation <= colliderAngle && colliderAngle <= rightAngle + rotation)
                    collidersInCone.Add(collider);
            }

            return collidersInCone.ToArray();
        }
    }
}
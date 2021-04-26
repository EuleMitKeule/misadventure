using UnityEngine;

namespace HotlineHyrule.Weapons.Projectiles
{
    public class ProjectileData : ScriptableObject
    {
        [SerializeField] public int damage;
        /// <summary>
        /// Layermask that contains layers that count as impacts.
        /// </summary>
        [SerializeField] public LayerMask impactMask;
        /// <summary>
        /// Offsets the start point if the impact raycast.
        /// </summary>
        [SerializeField] public float impactRaycastOffset;
        [SerializeField] public bool isSticky;
        [SerializeField] public int penetrations;
    }
}
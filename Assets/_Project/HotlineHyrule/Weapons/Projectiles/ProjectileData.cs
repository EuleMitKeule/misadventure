using UnityEngine;

namespace HotlineHyrule.Weapons.Projectiles
{
    public class ProjectileData : ScriptableObject
    {
        /// <summary>
        /// The damage the projectile deals to entities on impact.
        /// </summary>
        [SerializeField] public int damage;
        /// <summary>
        /// Layers that count as impact.
        /// </summary>
        [SerializeField] public LayerMask impactMask;
        /// <summary>
        /// Offsets the start point of the impact raycast.
        /// </summary>
        [SerializeField] public float impactRaycastOffset;
        /// <summary>
        /// Whether the projectile should stick to entities on impact.
        /// </summary>
        [SerializeField] public bool isSticky;
        /// <summary>
        /// How often the projectile can penetrate entities.
        /// </summary>
        [SerializeField] public int penetrations;
        /// <summary>
        /// The particle system to spawn on impact.
        /// </summary>
        [SerializeField] public GameObject impactParticleSystem;
    }
}
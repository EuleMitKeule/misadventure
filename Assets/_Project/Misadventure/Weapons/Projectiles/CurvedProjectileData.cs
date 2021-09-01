using UnityEngine;

namespace HotlineHyrule.Weapons.Projectiles
{
    [CreateAssetMenu(menuName = "Weapon/New Curved Projectile")]
    public class CurvedProjectileData : ProjectileData
    {
        /// <summary>
        /// The distance the projectile should travel.
        /// </summary>
        [SerializeField] public float range = 10f;
        /// <summary>
        /// The time in which the projectile should travel its distance.
        /// </summary>
        [SerializeField] public float flightTime = 1f;
        /// <summary>
        /// The minimum speed amount that counts as movement.
        /// </summary>
        [Range(0.01f, 2.5f)] [SerializeField] public float movementThreshold = 0.5f;
    }
}
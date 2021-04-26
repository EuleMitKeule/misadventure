using UnityEngine;

namespace HotlineHyrule.Weapons.Projectiles
{
    [CreateAssetMenu(menuName = "Weapon/New Linear Projectile")]
    public class LinearProjectileData : ProjectileData
    {
        /// <summary>
        /// The speed the shot projectile travels at.
        /// </summary>
        [SerializeField] public float movementSpeed;
    }
}
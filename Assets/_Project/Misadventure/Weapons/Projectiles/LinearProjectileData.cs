using UnityEngine;

namespace Misadventure.Weapons.Projectiles
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
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    /// <summary>
    /// Determines the properties of a ranged weapon.
    /// </summary>
    [CreateAssetMenu(menuName = "Weapons/New Ranged Weapon")]
    public class RangedWeaponData : WeaponData
    {
        /// <summary>
        /// The speed a shot bullet travels at.
        /// </summary>
        [SerializeField] public float bulletSpeed;
        /// <summary>
        /// The prefab used for spawning a bullet.
        /// </summary>
        [SerializeField] public GameObject bulletPrefab;
        /// <summary>
        /// The position the projectile is shot from.
        /// </summary>
        [SerializeField] public Vector2 spawnPosition;
    }
}

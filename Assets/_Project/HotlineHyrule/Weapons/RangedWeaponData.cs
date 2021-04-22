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
        /// The radius of the look target's deadzone around the player.
        /// </summary>
        [SerializeField] public float deadzoneRadius;

        /// <summary>
        /// The prefab used for spawning a projectile.
        /// </summary>
        [SerializeField] public GameObject projectilePrefab;

        /// <summary>
        /// The position the projectile is shot from.
        /// </summary>
        [SerializeField] public Vector2 spawnPosition;
    }
}

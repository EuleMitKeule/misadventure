using UnityEngine;

namespace Misadventure.Weapons
{
    /// <summary>
    /// Determines the properties of a ranged weapon.
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/New Ranged Weapon")]
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

        /// <summary>
        /// The sound that is played when the weapon is fired.
        /// </summary>
        [SerializeField] public AudioClip weaponFiredSound;

        [Header("Number of projectiles")]
        [SerializeField] public int projectileCount = 1;
        [SerializeField] public float projectileAngle;
        [SerializeField] public float projectileAngleOffset;
        [SerializeField] public float projectileDelay;
        [SerializeField] public bool flip;
    }
}

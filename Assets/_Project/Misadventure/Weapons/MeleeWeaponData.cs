using UnityEngine;

namespace Misadventure.Weapons
{
    [CreateAssetMenu(menuName = "Weapon/New Melee Weapon")]
    public class MeleeWeaponData : WeaponData
    {
        /// <summary>
        /// The damage value to deal to entites on impact.
        /// </summary>
        [SerializeField] public int damage;
    }
}
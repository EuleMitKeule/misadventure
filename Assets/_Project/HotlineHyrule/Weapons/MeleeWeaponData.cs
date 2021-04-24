using System.Collections.Generic;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    [CreateAssetMenu(menuName = "Weapons/New Melee Weapon")]
    public class MeleeWeaponData : WeaponData
    {
        /// <summary>
        /// The damage a hit of the weapon deals.
        /// </summary>
        [SerializeField] public int damage;
    }
}
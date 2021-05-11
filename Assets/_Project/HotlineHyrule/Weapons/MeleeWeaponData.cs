using System.Collections.Generic;
using UnityEngine;

namespace HotlineHyrule.Weapons
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
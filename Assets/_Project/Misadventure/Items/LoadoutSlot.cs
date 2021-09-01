using System;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Items
{
    [Serializable]
    public class LoadoutSlot
    {
        /// <summary>
        /// The equipped weapon.
        /// </summary>
        [SerializeField] public WeaponData weaponData;
        /// <summary>
        /// The number of charges left on the weapon.
        /// </summary>
        [SerializeField] public int weaponCharges;
    }
}
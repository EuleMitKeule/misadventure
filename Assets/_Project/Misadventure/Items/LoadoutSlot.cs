using System;
using Misadventure.Weapons;
using UnityEngine;

namespace Misadventure.Items
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
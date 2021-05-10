using System;
using HotlineHyrule.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Entities
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
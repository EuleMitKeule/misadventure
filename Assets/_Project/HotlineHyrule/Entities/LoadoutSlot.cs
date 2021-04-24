using System;
using HotlineHyrule.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Entities
{
    [Serializable]
    public class LoadoutSlot
    {
        [SerializeField] public WeaponData weaponData;
        [SerializeField] public int weaponCharges;
    }
}
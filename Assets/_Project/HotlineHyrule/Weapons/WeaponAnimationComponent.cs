using System;
using HotlineHyrule.Entities;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    public class WeaponAnimationComponent : MonoBehaviour
    {
        WeaponComponent WeaponComponent { get; set; }
        LoadoutComponent LoadoutComponent { get; set; }
        
        void Awake()
        {
            WeaponComponent = GetComponentInParent<WeaponComponent>();
            LoadoutComponent = GetComponentInParent<LoadoutComponent>();
        }

        public void PerformRangedAttack()
        {
            if (!WeaponComponent) return;
            
            WeaponComponent.PerformRangedAttack();
        }

        public void Unequip()
        {
            if (!LoadoutComponent) return;

            LoadoutComponent.Unequip();
        }
    }
}
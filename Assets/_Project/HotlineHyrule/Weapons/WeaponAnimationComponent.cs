using System;
using HotlineHyrule.Entities;
using HotlineHyrule.Items;
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

        /// <summary>
        /// Performs a ranged attack.
        /// </summary>
        public void PerformRangedAttack()
        {
            if (!WeaponComponent) return;
            
            WeaponComponent.PerformRangedAttack();
        }

        /// <summary>
        /// Performs a melee attack.
        /// </summary>
        public void PerformMeleeAttack()
        {
            if (!WeaponComponent) return;

            WeaponComponent.PerformMeleeAttack();
        }

        /// <summary>
        /// Performs a conjuring attack.
        /// </summary>
        public void PerformConjuringAttack()
        {
            if (!WeaponComponent) return;

            WeaponComponent.PerformConjuringAttack();
        }
        
        public void PerformTargetingAttack()
        {
            if (!WeaponComponent) return;

            WeaponComponent.PerformTargetingAttack();
        }

        /// <summary>
        /// Unequips the current weapon without dropping it.
        /// </summary>
        public void Unequip()
        {
            if (!LoadoutComponent) return;

            LoadoutComponent.Unequip(false);
        }

        /// <summary>
        /// Unequips and drops the current weapon.
        /// </summary>
        public void UnequipAndDrop()
        {
            if (!LoadoutComponent) return;

            LoadoutComponent.Unequip(true);
        }
    }
}
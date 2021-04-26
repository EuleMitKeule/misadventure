using System;
using HotlineHyrule.Entities;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    public class WeaponAnimationComponent : MonoBehaviour
    {
        LoadoutComponent LoadoutComponent { get; set; }
        
        void Awake()
        {
            LoadoutComponent = GetComponentInParent<LoadoutComponent>();
        }

        public void Unequip()
        {
            if (!LoadoutComponent) return;

            LoadoutComponent.Unequip();
        }
    }
}
using System.Collections.Generic;
using HotlineHyrule.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Runtime.ExceptionServices;
using HotlineHyrule.Items;

namespace HotlineHyrule.Entities
{
    public class LoadoutComponent : MonoBehaviour
    {
        [SerializeField] MeleeWeaponData meleeWeaponData;
        [SerializeField] RangedWeaponData rangedWeaponData;
        [SerializeField] InputAction meleeWeaponAction;
        [SerializeField] InputAction rangedWeaponAction;
        WeaponData[] Weapons => new WeaponData[] {meleeWeaponData, rangedWeaponData};
            
        WeaponComponent WeaponComponent { get; set; }

        void Awake()
        {
            WeaponComponent = GetComponentInChildren<WeaponComponent>();

            meleeWeaponAction.started += OnButtonMeleeWeapon;
            rangedWeaponAction.started += OnButtonRangedWeapon;
            
            meleeWeaponAction.Enable();
            rangedWeaponAction.Enable();
        }

        void OnButtonMeleeWeapon(InputAction.CallbackContext context) => WeaponComponent.SetWeapon(meleeWeaponData);

        void OnButtonRangedWeapon(InputAction.CallbackContext context) => WeaponComponent.SetWeapon(rangedWeaponData);

        public void PickUpWeapon(WeaponData weaponData)
        {
            switch (weaponData)
            {
                case MeleeWeaponData newMeleeWeaponData:
                    meleeWeaponData = newMeleeWeaponData;
                    WeaponComponent.SetWeapon(meleeWeaponData);
                    break;
                case RangedWeaponData newRangedWeaponData:
                    rangedWeaponData = newRangedWeaponData;
                    WeaponComponent.SetWeapon(rangedWeaponData);
                    break;
            }
        }
    }
}

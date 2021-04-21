using HotlineHyrule.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Entities
{
    public class InventoryComponent : MonoBehaviour
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

        void OnButtonMeleeWeapon(InputAction.CallbackContext obj) => WeaponComponent.SetWeapon(meleeWeaponData);

        void OnButtonRangedWeapon(InputAction.CallbackContext obj) => WeaponComponent.SetWeapon(rangedWeaponData);
    }
}
using System.Collections.Generic;
using HotlineHyrule.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace HotlineHyrule.Entities
{
    public class InventoryComponent : MonoBehaviour
    {
        [SerializeField] float pickupRadius;
        [SerializeField] MeleeWeaponData meleeWeaponData;
        [SerializeField] RangedWeaponData rangedWeaponData;
        [SerializeField] LayerMask itemMask;
        [SerializeField] InputAction meleeWeaponAction;
        [SerializeField] InputAction rangedWeaponAction;
        [SerializeField] InputAction pickupAction;

        Collider2D[] OverlappingItems => Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemMask);
        Collider2D ClosestItem => OverlappingItems.OrderBy(element => (element.transform.position - transform.position).magnitude).First();
        WeaponData[] Weapons => new WeaponData[] {meleeWeaponData, rangedWeaponData};
            
        WeaponComponent WeaponComponent { get; set; }

        void Awake()
        {
            WeaponComponent = GetComponentInChildren<WeaponComponent>();

            meleeWeaponAction.started += OnButtonMeleeWeapon;
            rangedWeaponAction.started += OnButtonRangedWeapon;
            pickupAction.started += OnButtonPickup;
            
            meleeWeaponAction.Enable();
            rangedWeaponAction.Enable();
            pickupAction.Enable();
        }

        void OnButtonMeleeWeapon(InputAction.CallbackContext context) => WeaponComponent.SetWeapon(meleeWeaponData);

        void OnButtonRangedWeapon(InputAction.CallbackContext context) => WeaponComponent.SetWeapon(rangedWeaponData);

        void OnButtonPickup(InputAction.CallbackContext context)
        {
            var closestItemComponent = ClosestItem.GetComponent<ItemComponent>();
            if (!closestItemComponent) return;
            var data = closestItemComponent.data;

            switch (data)
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
            
            Destroy(closestItemComponent.gameObject);
        }
    }
}

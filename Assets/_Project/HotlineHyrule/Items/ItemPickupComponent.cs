using System.Linq;
using HotlineHyrule.Entities;
using HotlineHyrule.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Items
{
    public class ItemPickupComponent : MonoBehaviour
    {
        [SerializeField] float pickupRadius;
        [SerializeField] LayerMask itemMask;
        [SerializeField] InputAction pickupAction;

        Collider2D[] OverlappingItems => Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemMask);
        Collider2D ClosestItem => OverlappingItems.OrderBy(element => (element.transform.position - transform.position).magnitude).FirstOrDefault();

        HealthComponent HealthComponent { get; set; }
        LoadoutComponent LoadoutComponent { get; set; }
        IMovementComponent MovementComponent { get; set; }
        WeaponComponent WeaponComponent { get; set; }

        void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
            LoadoutComponent = GetComponent<LoadoutComponent>();
            MovementComponent = GetComponent<IMovementComponent>();
            WeaponComponent = GetComponentInChildren<WeaponComponent>();
            
            pickupAction.started += OnButtonPickup;
            
            pickupAction.Enable();
        }

        void OnButtonPickup(InputAction.CallbackContext context)
        {
            var closestItem = ClosestItem;
            if (!closestItem) return;
            var closestItemComponent = closestItem.GetComponent<ItemComponent>();
            if (!closestItemComponent) return;
            var itemDatas = closestItemComponent.itemDatas;

            foreach (var itemData in itemDatas)
            {
                switch (itemData)
                {
                    case WeaponData weaponData:
                        if (!LoadoutComponent) continue;
                        var droppedWeaponComponent = closestItem.GetComponent<DroppedWeaponComponent>();
                        LoadoutComponent.PickUpWeapon(weaponData, droppedWeaponComponent);
                        break;
                    case HealthItemData healthItemData:
                        if (!HealthComponent) continue;
                        HealthComponent.Consume(healthItemData);        
                        break;
                    case AttackItemData attackItemData:
                        if (!WeaponComponent) continue;
                        WeaponComponent.Consume(attackItemData);
                        break;
                    case MovementItemData movementItemData:
                        if (MovementComponent == null) continue;
                        MovementComponent.Consume(movementItemData);
                        break;
                }
            }
            
            Destroy(closestItemComponent.gameObject);
        }
    }
}
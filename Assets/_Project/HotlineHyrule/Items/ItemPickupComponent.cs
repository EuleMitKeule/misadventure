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
        Collider2D ClosestItem => OverlappingItems.OrderBy(element => (element.transform.position - transform.position).magnitude).First();

        HealthComponent HealthComponent { get; set; }
        LoadoutComponent LoadoutComponent { get; set; }

        void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
            LoadoutComponent = GetComponent<LoadoutComponent>();
            
            pickupAction.started += OnButtonPickup;
            
            pickupAction.Enable();
        }

        void OnButtonPickup(InputAction.CallbackContext context)
        {
            var closestItemComponent = ClosestItem.GetComponent<ItemComponent>();
            if (!closestItemComponent) return;
            var itemDatas = closestItemComponent.itemDatas;

            foreach (var itemData in itemDatas)
            {
                switch (itemData)
                {
                    case WeaponData weaponData:
                        if (!LoadoutComponent) continue;
                        LoadoutComponent.PickUpWeapon(weaponData);
                        break;
                    case HealthItemData healthItemData:
                        if (!HealthComponent) continue;
                        HealthComponent.Consume(healthItemData);        
                        break;
                }
            }
            
            Destroy(closestItemComponent.gameObject);
        }
    }
}
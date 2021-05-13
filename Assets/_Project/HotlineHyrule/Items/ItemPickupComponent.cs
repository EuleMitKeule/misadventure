using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using HotlineHyrule.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Items
{
    public class ItemPickupComponent : MonoBehaviour
    {
        /// <summary>
        /// The range in which items can be picked up.
        /// </summary>
        [Header("General")]
        [SerializeField] float pickupRadius;
        /// <summary>
        /// Which layers count as item.
        /// </summary>
        [SerializeField] LayerMask itemMask;
        /// <summary>
        /// The input action to pickup items.
        /// </summary>
        [Header("Input")]
        [SerializeField] InputAction pickupAction;

        /// <summary>
        /// List of items that are currently in range.
        /// </summary>
        Collider2D[] OverlappingItems =>
            Physics2D.OverlapCircleAll(transform.position, pickupRadius, itemMask);
        /// <summary>
        /// The item that is currently closest.
        /// </summary>
        Collider2D ClosestItem =>
            OverlappingItems
            .OrderBy(element => (element.transform.position - transform.position).magnitude)
            .FirstOrDefault();

        HealthComponent HealthComponent { get; set; }
        LoadoutComponent LoadoutComponent { get; set; }
        IMovementComponent MovementComponent { get; set; }
        WeaponComponent WeaponComponent { get; set; }

        void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
            LoadoutComponent = GetComponent<LoadoutComponent>();
            MovementComponent = GetComponent<IMovementComponent>();
            WeaponComponent = GetComponent<WeaponComponent>();
            
            pickupAction.started += OnButtonPickup;
            
            pickupAction.Enable();

            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            pickupAction.Disable();
        }

        void OnButtonPickup(InputAction.CallbackContext context)
        {
            var closestItem = ClosestItem;
            if (!closestItem) return;

            var closestItemComponent = closestItem.GetComponent<ItemComponent>();
            if (!closestItemComponent) return;

            ConsumeItem(closestItemComponent);
        }

        /// <summary>
        /// Applies the item's effects and destroys the item object.
        /// </summary>
        /// <param name="itemComponent"></param>
        void ConsumeItem(ItemComponent itemComponent)
        {
            var items = itemComponent.itemDatas;

            foreach (var itemData in items)
            {
                if (itemData is ConsumableItemData consumableItemData) SpawnParticleSystem(consumableItemData);

                switch (itemData)
                {
                    case WeaponData weaponData:
                        if (!LoadoutComponent) continue;
                        var droppedWeaponComponent = itemComponent.GetComponent<DroppedWeaponComponent>();
                        LoadoutComponent.Equip(weaponData, droppedWeaponComponent.weaponCharges);
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

            Destroy(itemComponent.gameObject);
        }

        /// <summary>
        /// Spawns the item's particle systems.
        /// </summary>
        /// <param name="consumableItemData"></param>
        void SpawnParticleSystem(ConsumableItemData consumableItemData)
        {
            if (!consumableItemData.consumeParticleSystemPrefab) return;
            Instantiate(consumableItemData.consumeParticleSystemPrefab, transform);
        }
    }
}
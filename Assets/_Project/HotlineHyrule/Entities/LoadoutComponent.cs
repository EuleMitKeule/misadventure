using System;
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
        [SerializeField] WeaponData defaultWeapon;
        [SerializeField] List<LoadoutSlot> loadoutSlots;
        [SerializeField] InputAction changeWeaponAction;
        
        public LoadoutSlot CurrentLoadoutSlot { get; set; }
        int CurrentLoadoutSlotIndex => loadoutSlots.IndexOf(CurrentLoadoutSlot);
        int NextLoadoutSlotIndex => (CurrentLoadoutSlotIndex + 1) % loadoutSlots.Count;
        int PreviousLoadoutSlotIndex => (CurrentLoadoutSlotIndex + loadoutSlots.Count - 1) % loadoutSlots.Count;
            
        WeaponComponent WeaponComponent { get; set; }

        void Awake()
        {
            WeaponComponent = GetComponentInChildren<WeaponComponent>();

            if (WeaponComponent)
            {
                WeaponComponent.AttackStarted += OnAttackStarted;
                WeaponComponent.AttackFinished += OnAttackFinished;
            }

            for (var i = 0; i < loadoutSlots.Count; i++)
            {
                var loadoutSlot = loadoutSlots[i];

                if (!loadoutSlot.weaponData) loadoutSlot.weaponData = defaultWeapon;
            }

            ChangeSlot(0);

            changeWeaponAction.started += _ => 
                ChangeSlot(changeWeaponAction.ReadValue<float>() > 0 ? NextLoadoutSlotIndex : PreviousLoadoutSlotIndex);
            changeWeaponAction.Enable();
        }

        void ChangeSlot(int slotIndex)
        {
            CurrentLoadoutSlot = loadoutSlots[slotIndex];
            Apply();
        }

        void Apply() => WeaponComponent.SetWeapon(CurrentLoadoutSlot.weaponData);

        void OnAttackStarted(object sender, EventArgs e)
        {
            if (!CurrentLoadoutSlot.weaponData) return;
            if (CurrentLoadoutSlot.weaponData.hasInfiniteCharges) return;
            CurrentLoadoutSlot.weaponCharges -= 1;
        }

        void OnAttackFinished(object sender, EventArgs e)
        {
            if (!CurrentLoadoutSlot.weaponData) return;
            if (CurrentLoadoutSlot.weaponData.hasInfiniteCharges) return;
            if (CurrentLoadoutSlot.weaponCharges <= 0)
            {
                CurrentLoadoutSlot.weaponData = defaultWeapon;
                ChangeSlot(CurrentLoadoutSlotIndex);
            }
        }

        public void Equip(WeaponData newWeaponData, DroppedWeaponComponent newDroppedWeaponComponent)
        {
            if (CurrentLoadoutSlot.weaponData.droppedWeaponPrefab)
            {
                var droppedWeaponObject =
                    Instantiate(CurrentLoadoutSlot.weaponData.droppedWeaponPrefab, transform.position, Quaternion.identity);
                var droppedWeaponComponent = droppedWeaponObject.GetComponent<DroppedWeaponComponent>();
                if (droppedWeaponComponent) droppedWeaponComponent.weaponCharges = CurrentLoadoutSlot.weaponCharges;
            }
            
            CurrentLoadoutSlot.weaponData = newWeaponData;
            CurrentLoadoutSlot.weaponCharges = newDroppedWeaponComponent.weaponCharges;
            
            ChangeSlot(CurrentLoadoutSlotIndex);
        }

        public void Unequip(int slotIndex = -1)
        {
            var isValidIndex = slotIndex >= 0 && slotIndex < loadoutSlots.Count;
            loadoutSlots[isValidIndex ? slotIndex : CurrentLoadoutSlotIndex].weaponData = defaultWeapon;

            Apply();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Misadventure.Level;
using Misadventure.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Misadventure.Items
{
    public class LoadoutComponent : MonoBehaviour
    {
        /// <summary>
        /// The weapon representing not having a weapon equipped.
        /// </summary>
        [Header("General")]
        [SerializeField] WeaponData defaultWeapon;
        /// <summary>
        /// List of slots in which weapons can be equipped.
        /// </summary>
        [SerializeField] public List<LoadoutSlot> loadoutSlots;
        /// <summary>
        /// The input action for changing the selected weapon slot.
        /// </summary>
        [Header("Input")]
        [SerializeField]
        public InputAction changeWeaponAction;

        /// <summary>
        /// The currently selected weapon slot.
        /// </summary>
        public LoadoutSlot CurrentLoadoutSlot { get; set; }
        /// <summary>
        /// The index of the currently selected weapon slot.
        /// </summary>
        int CurrentLoadoutSlotIndex => loadoutSlots.IndexOf(CurrentLoadoutSlot);
        /// <summary>
        /// The index of the next weapon slot.
        /// </summary>
        int NextLoadoutSlotIndex => (CurrentLoadoutSlotIndex + 1) % loadoutSlots.Count;
        /// <summary>
        /// The index of the previous weapon slot.
        /// </summary>
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

            foreach (var loadoutSlot in loadoutSlots.Where(loadoutSlot => !loadoutSlot.weaponData))
            {
                loadoutSlot.weaponData = defaultWeapon;
            }

            ChangeSlot(0);

            changeWeaponAction.started += _ => 
                ChangeSlot(changeWeaponAction.ReadValue<float>() > 0 ? NextLoadoutSlotIndex : PreviousLoadoutSlotIndex);
            changeWeaponAction.Enable();

            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            if (!e.PlayerStateData) return;

            loadoutSlots = e.PlayerStateData.loadoutSlots;
            CurrentLoadoutSlot = e.PlayerStateData.currentLoadoutSlot;
            Apply();
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            changeWeaponAction.Disable();

            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        /// <summary>
        /// Changes the currently equipped weapon slot.
        /// </summary>
        /// <param name="slotIndex">The index of the slot to change to.</param>
        void ChangeSlot(int slotIndex)
        {
            CurrentLoadoutSlot = loadoutSlots[slotIndex];
            Apply();
        }

        /// <summary>
        /// Equips the weapon inside the selected loadout slot.
        /// </summary>
        void Apply()
        {
            WeaponComponent.SetWeapon(CurrentLoadoutSlot.weaponData);
        }

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

        /// <summary>
        /// Equips a weapon to the currently selected weapon slot.
        /// </summary>
        /// <param name="newWeaponData">The weapon data to equip.</param>
        /// <param name="weaponCharges">The number of weapon charges left on the weapon.</param>
        public void Equip(WeaponData newWeaponData, int weaponCharges)
        {
            DropWeapon();
            
            CurrentLoadoutSlot.weaponData = newWeaponData;
            CurrentLoadoutSlot.weaponCharges = weaponCharges;
            
            ChangeSlot(CurrentLoadoutSlotIndex);
        }

        /// <summary>
        /// Spawns the droppped weapon prefab of the currently equipped weapon.
        /// </summary>
        public void DropWeapon()
        {
            if (!CurrentLoadoutSlot.weaponData.ItemPrefab) return;
            if (CurrentLoadoutSlot.weaponData == defaultWeapon) return;
            
            var droppedWeaponObject =
                Instantiate(CurrentLoadoutSlot.weaponData.ItemPrefab, transform.position, transform.rotation);
            var droppedWeaponComponent = droppedWeaponObject.GetComponent<DroppedWeaponComponent>();
            if (droppedWeaponComponent) droppedWeaponComponent.weaponCharges = CurrentLoadoutSlot.weaponCharges;
        }

        /// <summary>
        /// Unequips the currently selected weapon slot.
        /// </summary>
        /// <param name="shouldDropWeapon">Whether the weapon should be dropped.</param>
        public void Unequip(bool shouldDropWeapon)
        {
            if (shouldDropWeapon) DropWeapon();

            loadoutSlots[CurrentLoadoutSlotIndex].weaponData = defaultWeapon;

            Apply();
        }
    }
}

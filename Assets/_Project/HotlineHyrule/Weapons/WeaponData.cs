using HotlineHyrule.Extensions;
using HotlineHyrule.Items;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    /// <summary>
    /// Determines the properties of a weapon.
    /// </summary>
    public abstract class WeaponData : ItemData
    {
        /// <summary>
        /// Multiplies the player's movement speed.
        /// </summary>
        [SerializeField] public float movementFactor = 1f;
        /// <summary>
        /// The amount of time the to slow the entity after attacking.
        /// </summary>
        [Range(0f, 1f)] [SerializeField] public float slowTimeWindow;
        /// <summary>
        /// The maximum amount of attacks performed per second.
        /// </summary>
        [SerializeField] public float attackRate = 1f;
        /// <summary>
        /// The weapon object associated with this weapon.
        /// </summary>
        [SerializeField] public GameObject weaponPrefab;
        /// <summary>
        /// Whether the weapon is limited by charges.
        /// </summary>
        [SerializeField] public bool hasInfiniteCharges;
        /// <summary>
        /// How often the weapon can be used.
        /// </summary>
        [SerializeField] public int weaponCharges;
        /// <summary>
        /// The percentage amount the weapon charges are randomized.
        /// </summary>
        [Range(0f, 1f)] [SerializeField] public float chargeRandomness;

        public override string ItemName => name.Replace("weapon_", "");
        protected override bool IsItemNameReadOnly => true;
    }
}
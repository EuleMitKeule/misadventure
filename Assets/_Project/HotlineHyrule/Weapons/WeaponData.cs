using HotlineHyrule.Items;
using UnityEditor;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    /// <summary>
    /// Determines the properties of a weapon.
    /// </summary>
    public class WeaponData : ItemData
    {
        /// <summary>
        /// Multiplies the player's movement speed.
        /// </summary>
        [SerializeField] public float movementFactor;
        [Range(0f, 1f)] [SerializeField] public float slowTimeWindow;
        /// <summary>
        /// The maximum amount of attacks performed per second.
        /// </summary>
        [SerializeField] public float attackRate;
        [SerializeField] public GameObject weaponPrefab;
        [SerializeField] public GameObject droppedWeaponPrefab;
        [SerializeField] public bool hasInfiniteCharges;
        [SerializeField] public int weaponCharges;
        [Range(0f, 1f)] [SerializeField] public float chargeRandomness;
    }
}
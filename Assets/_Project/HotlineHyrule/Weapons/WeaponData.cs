using UnityEngine;

namespace HotlineHyrule.Weapons
{
    /// <summary>
    /// Determines the properties of a weapon.
    /// </summary>
    public class WeaponData : ScriptableObject
    {
        /// <summary>
        /// The name of the weapon.
        /// </summary>
        [SerializeField] public string weaponName;
        /// <summary>
        /// Multiplies the player's movement speed.
        /// </summary>
        [SerializeField] public float movementFactor;
        
        [Range(0f, 1f)] [SerializeField] public float slowTimeWindow;
        /// <summary>
        /// The maximum amount of attacks performed per second.
        /// </summary>
        [SerializeField] public float attackRate;
        /// <summary>
        /// The damage a hit of the weapon deals.
        /// </summary>
        [SerializeField] public int damage;
        [SerializeField] public GameObject weaponPrefab;
        [SerializeField] public GameObject weaponDroppedPrefab;
    }
}
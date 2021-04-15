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
        /// The radius of the look target's deadzone around the player.
        /// </summary>
        [SerializeField] public float deadzoneRadius;
        /// <summary>
        /// The maximum amount of attacks performed per second.
        /// </summary>
        [SerializeField] public float attackRate;
        /// <summary>
        /// The sprite of the weapon.
        /// </summary>
        [SerializeField] public Sprite weaponSprite;
    }
}
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
        /// The maximum amount of attacks performed per second.
        /// </summary>
        [SerializeField] public float attackRate;
        /// <summary>
        /// The sprite of the weapon.
        /// </summary>
        [SerializeField] public Sprite weaponSprite;
        [SerializeField] Animation attackAnimation;
        [SerializeField] GameObject attackParticleSystemPrefab;
    }
}
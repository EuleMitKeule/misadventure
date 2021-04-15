using UnityEngine;

namespace HotlineHyrule.Weapons
{
    public class WeaponData : ScriptableObject
    {
        [SerializeField] public string _weaponName;
        /// <summary>
        /// The radius of the look target's deadzone around the player.
        /// </summary>
        [SerializeField] public float _deadzoneRadius;
        [SerializeField] public float _fireSpeed;
    }
}
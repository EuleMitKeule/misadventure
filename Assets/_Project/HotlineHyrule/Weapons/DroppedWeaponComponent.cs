using UnityEngine;

namespace HotlineHyrule.Weapons
{
    public class DroppedWeaponComponent : MonoBehaviour
    {
        /// <summary>
        /// The number of charges left on this weapon.
        /// </summary>
        [SerializeField] public int weaponCharges;
    }
}
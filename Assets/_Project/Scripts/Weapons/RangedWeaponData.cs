using UnityEngine;

namespace Scripts.Weapons
{
    [CreateAssetMenu(menuName = "Weapons/New Ranged Weapon")]
    public class RangedWeaponData : WeaponData
    {
        [SerializeField] public float fireSpeed;
        [SerializeField] public float bulletSpeed;
        [SerializeField] public GameObject bulletPrefab;
        [SerializeField] public Sprite weaponSprite;
    }
}

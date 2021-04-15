using UnityEngine;

namespace HotlineHyrule.Weapons
{
    [CreateAssetMenu(menuName = "Weapons/New Ranged Weapon")]
    public class RangedWeaponData : WeaponData
    {
        [SerializeField] public float _bulletSpeed;
        [SerializeField] public GameObject _bulletPrefab;
        [SerializeField] public Sprite _weaponSprite;
    }
}

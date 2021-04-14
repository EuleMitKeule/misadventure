using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Weapons
{
    public class WeaponComponent : MonoBehaviour
    {
        [SerializeField] public WeaponData _weaponData;
        [SerializeField] InputAction _fireAction;

        float LastFireTime { get; set; }

        bool IsFiring => _fireAction.ReadValue<float>() != 0f;
        bool CanFire => Time.time >= LastFireTime + 1 / _weaponData._fireSpeed;

        Transform Transform { get; set; }

        void Awake()
        {
            Transform = transform;

            _fireAction.Enable();
        }

        void Update()
        {
            if (IsFiring) FireWeapon();
        }

        public void SetWeapon(WeaponData weaponData) => _weaponData = weaponData;

        void FireWeapon()
        {
            if (!CanFire) return;
            LastFireTime = Time.time;

            if (_weaponData is RangedWeaponData rangedWeaponData)
            {
                var bulletObject = Instantiate(rangedWeaponData._bulletPrefab, Transform.position, Transform.rotation);
                var bulletRigidbody = bulletObject.GetComponent<Rigidbody2D>();

                bulletRigidbody.velocity = Transform.up * rangedWeaponData._bulletSpeed;

                Destroy(bulletObject, 10f);
            }
        }
    }
}

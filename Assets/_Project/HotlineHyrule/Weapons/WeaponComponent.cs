using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Weapons
{
    /// <summary>
    /// Handles the behavior of the weapon it's attached to.
    /// </summary>
    public class  WeaponComponent : MonoBehaviour
    {
        /// <summary>
        /// The weapon data corresponding to this weapon.
        /// </summary>
        [SerializeField] public WeaponData weaponData;
        /// <summary>
        /// The attack input action.
        /// </summary>
        [SerializeField] InputAction attackAction;

        /// <summary>
        /// The last point in time the weapon was used at.
        /// </summary>
        float LastAttackTime { get; set; }

        /// <summary>
        /// Whether the attack input is currently being registered.
        /// </summary>
        bool IsAttacking => attackAction.ReadValue<float>() != 0f;
        /// <summary>
        /// Whether enough time has passed since the last usage for the weapon to be used again.
        /// </summary>
        bool CanAttack => Time.time >= LastAttackTime + 1 / weaponData.attackRate;

        Transform Transform { get; set; }

        void Awake()
        {
            Transform = transform;

            attackAction.Enable();
        }

        void Update()
        {
            if (IsAttacking) PerformAttack();
        }

        public void SetWeapon(WeaponData weaponData) => this.weaponData = weaponData;

        /// <summary>
        /// Performs an attack if possible.
        /// </summary>
        void PerformAttack()
        {
            if (!CanAttack) return;
            LastAttackTime = Time.time;

            if (weaponData is RangedWeaponData rangedWeaponData)
            {
                var bulletObject = Instantiate(rangedWeaponData.bulletPrefab, Transform.position, Transform.rotation);
                var bulletRigidbody = bulletObject.GetComponent<Rigidbody2D>();

                bulletRigidbody.velocity = Transform.up * rangedWeaponData.bulletSpeed;

                Destroy(bulletObject, 10f);
            }
        }
    }
}

using HotlineHyrule.Entities;
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
        /// Whether the weapon component's parent is the player.
        /// </summary>
        bool IsPlayer => GetComponentInParent<PlayerComponent>();
        /// <summary>
        /// Whether the attack input is currently being registered.
        /// </summary>
        bool IsAttacking => attackAction.ReadValue<float>() != 0f;
        /// <summary>
        /// Whether enough time has passed since the last usage for the weapon to be used again.
        /// </summary>
        bool CanAttack => Time.time >= LastAttackTime + 1 / weaponData.attackRate;

        Transform Transform { get; set; }
        SpriteRenderer SpriteRenderer { get; set; }

        void Awake()
        {
            Transform = transform;
            SpriteRenderer = GetComponent<SpriteRenderer>();

            attackAction.Enable();
        }

        void Update()
        {
            if (IsAttacking) PerformAttack();
        }

        public void SetWeapon(WeaponData newWeaponData)
        {
            weaponData = newWeaponData;
            SpriteRenderer.sprite = newWeaponData.weaponSprite;
        }

        /// <summary>
        /// Performs an attack if possible.
        /// </summary>
        void PerformAttack()
        {
            if (!CanAttack) return;
            LastAttackTime = Time.time;

            if (weaponData is RangedWeaponData rangedWeaponData) PerformRangedAttack(rangedWeaponData);
        }

        /// <summary>
        /// Performs a ranged attack with the passed ranged weapon.
        /// </summary>
        /// <param name="rangedWeaponData">The ranged weapon to perform the attack with.</param>
        void PerformRangedAttack(RangedWeaponData rangedWeaponData)
        {
            var bulletObject = Instantiate(rangedWeaponData.bulletPrefab, Transform.position, Transform.rotation);

            bulletObject.SetActive(false);

            var bulletComponent = bulletObject.GetComponent<BulletComponent>();
            bulletComponent.impactMask = new LayerMask();
            bulletComponent.impactMask.value |= 1 << PhysicsLayer.WALL;
            bulletComponent.impactMask.value |= 1 << (IsPlayer ? PhysicsLayer.ENEMY : PhysicsLayer.PLAYER);

            bulletObject.SetActive(true);

            var bulletRigidbody = bulletObject.GetComponent<Rigidbody2D>();
            bulletRigidbody.velocity = Transform.up * rangedWeaponData.bulletSpeed;
        }
    }
}

using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using HotlineHyrule.Entities;
using HotlineHyrule.Extensions;
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
        bool IsPlayer => PlayerComponent;
        /// <summary>
        /// Whether the attack input is currently being registered.
        /// </summary>
        bool IsAttacking => attackAction.ReadValue<float>() != 0f;
        /// <summary>
        /// Whether enough time has passed since the last usage for the weapon to be used again.
        /// </summary>
        bool CanAttack => Time.time >= LastAttackTime + 1 / weaponData.attackRate;
        /// <summary>
        /// Whether the current weapon is a ranged one.
        /// </summary>
        public bool HasRangedWeapon => weaponData is RangedWeaponData;
        /// <summary>
        /// Whether the current weapon is a melee one.
        /// </summary>
        public bool HasMeleeWeapon => weaponData is MeleeWeaponData;
        /// <summary>
        /// The ranged weapon data of the ranged weapon.
        /// </summary>
        public RangedWeaponData RangedWeaponData => (RangedWeaponData)weaponData;
        MeleeWeaponData MeleeWeaponData => (MeleeWeaponData)weaponData;
        /// <summary>
        /// The offset of the projectile spawn position relative to the weapon position.
        /// </summary>
        Vector3 ProjectileSpawnOffset =>
            Transform.right * RangedWeaponData.spawnPosition.x +
            Transform.up * RangedWeaponData.spawnPosition.y;
        /// <summary>
        /// The spawn position of the projectile.
        /// </summary>
        Vector3 ProjectileSpawnPosition => ProjectileSpawnOffset + Transform.position;
        public event EventHandler<EventArgs> AttackFinished;
        
        GameObject WeaponObject { get; set; }
        Transform Transform { get; set; }
        SpriteRenderer SpriteRenderer { get; set; }
        Animator Animator { get; set; }
        Rigidbody2D PlayerRigidbody { get; set; }
        PlayerComponent PlayerComponent { get; set; }

        void Awake()
        {
            Transform = transform;
            SpriteRenderer = GetComponent<SpriteRenderer>();
            PlayerRigidbody = GetComponentsInParent<Rigidbody2D>()[1];
            PlayerComponent = GetComponentInParent<PlayerComponent>();

            AttackFinished += OnAttackFinished;

            if (weaponData) SetWeapon(weaponData);
            attackAction.Enable();
        }

        void Update()
        {
            if (IsAttacking) PerformAttack();
        }

        /// <summary>
        /// Sets the current weapon to the given one.
        /// </summary>
        /// <param name="newWeaponData"></param>
        public void SetWeapon(WeaponData newWeaponData)
        {
            if (WeaponObject) Destroy(WeaponObject);
            
            weaponData = newWeaponData;
            WeaponObject = Instantiate(weaponData.weaponPrefab, Transform);
            Animator = WeaponObject.GetComponent<Animator>();
        }

        /// <summary>
        /// Performs an attack if possible.
        /// </summary>
        void PerformAttack()
        {
            if (!CanAttack) return;
            LastAttackTime = Time.time;

            Invoke(nameof(InvokeAttackFinished), weaponData.slowTimeWindow / weaponData.attackRate);
            
            if (IsPlayer)
            {
                PlayerComponent.MovementFactor = weaponData.movementFactor;
            }
            
            Animator.SetTrigger("attack");
            
            if (HasRangedWeapon) PerformRangedAttack();
            else if (HasMeleeWeapon) PerformMeleeAttack();
        }

        /// <summary>
        /// Performs a ranged attack with the equipped ranged weapon.
        /// </summary>
        void PerformRangedAttack()
        {
            if (!HasRangedWeapon) return;
            
            var projectileObject = Instantiate(RangedWeaponData.projectilePrefab, ProjectileSpawnPosition, Transform.rotation);

            projectileObject.SetActive(false);

            var projectileComponent = projectileObject.GetComponent<ProjectileComponent>();
            projectileComponent.impactMask = new LayerMask();
            projectileComponent.impactMask.value |= 1 << PhysicsLayer.WALL;
            projectileComponent.impactMask.value |= 1 << (IsPlayer ? PhysicsLayer.ENEMY : PhysicsLayer.PLAYER);
            projectileComponent.ImpactDamage = weaponData.damage;

            projectileObject.SetActive(true);

            var animator = projectileObject.GetComponent<Animator>();
            if (animator) animator.SetTrigger("attack");

            var projectileRigidbody = projectileObject.GetComponent<Rigidbody2D>();
            if (projectileRigidbody)
            {
                var velocity = RangedWeaponData.projectileSpeed == 0f
                    ? PlayerRigidbody.velocity
                    : (Vector2)Transform.up * RangedWeaponData.projectileSpeed;
                projectileRigidbody.velocity = velocity;
            }
        }

        void PerformMeleeAttack()
        {
            if (!HasMeleeWeapon) return;
        }

        void OnAttackFinished(object sender, EventArgs e)
        {
            if (IsPlayer)
            {
                PlayerComponent.MovementFactor = 1f;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!HasMeleeWeapon) return;
            
            var healthComponent = other.GetComponent<HealthComponent>();
            if (!healthComponent) return;

            healthComponent.Health -= weaponData.damage;
        }

        void InvokeAttackFinished() => AttackFinished?.Invoke(this, EventArgs.Empty);
    }
}

using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using HotlineHyrule.Entities;
using HotlineHyrule.Extensions;
using HotlineHyrule.Items;
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

        float _attackSpeed;
        float _particleSimulationSpeed;
        float AttackSpeed
        {
            get => _attackSpeed;
            set
            {
                _attackSpeed = value;
                if (WeaponAnimator) WeaponAnimator.SetFloat("attackSpeed", _attackSpeed);
                if (ParticleSystem)
                {
                    var mainModule = ParticleSystem.main;
                    _particleSimulationSpeed = mainModule.simulationSpeed;
                    mainModule.simulationSpeed *= AttackSpeed;
                }
            }
        }

        float DamageFactor { get; set; }
        int DamageBonus { get; set; }
        /// <summary>
        /// Whether the weapon component's parent is the player.
        /// </summary>
        bool IsPlayer => PlayerComponent;
        /// <summary>
        /// Whether the attack input is currently being registered.
        /// </summary>
        bool IsRequestingAttack => attackAction.ReadValue<float>() != 0f;
        /// <summary>
        /// Whether enough time has passed since the last usage for the weapon to be used again.
        /// </summary>
        bool CanAttack => Time.time >= LastAttackTime + 1 / weaponData.attackRate / AttackSpeed;
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

        GameObject InstantiateProjectile => Instantiate(RangedWeaponData.projectilePrefab, ProjectileSpawnPosition,
            Transform.rotation);

        Rigidbody2D GetParentRigidbody
        {
            get
            {
                var rigidbodies = GetComponentsInParent<Rigidbody2D>();

                var parentRigidbody = Rigidbody ?
                    (rigidbodies.Length >= 2 ? rigidbodies[1] : null) :
                    (rigidbodies.Length >= 1) ? rigidbodies[0] : null;

                return parentRigidbody;
            }
        }

        public event EventHandler<EventArgs> AttackStarted;
        public event EventHandler<EventArgs> AttackFinished;
        
        GameObject WeaponObject { get; set; }
        Transform Transform { get; set; }
        SpriteRenderer SpriteRenderer { get; set; }
        Animator WeaponAnimator { get; set; }
        Rigidbody Rigidbody { get; set; }
        Rigidbody2D ParentRigidbody { get; set; }
        PlayerComponent PlayerComponent { get; set; }
        ParticleSystem ParticleSystem { get; set; }
        LoadoutComponent LoadoutComponent { get; set; }

        void Awake()
        {
            Transform = transform;
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody>();
            ParentRigidbody = GetParentRigidbody;
            PlayerComponent = GetComponentInParent<PlayerComponent>();
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
            LoadoutComponent = GetComponentInParent<LoadoutComponent>();

            AttackFinished += OnAttackFinished;

            ResetBuffs();
            
            if (weaponData) SetWeapon(weaponData);
            attackAction.Enable();
        }

        void Update()
        {
            if (IsRequestingAttack) PerformAttack();
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
            WeaponAnimator = WeaponObject.GetComponent<Animator>();
        }

        /// <summary>
        /// Performs an attack if possible.
        /// </summary>
        public void PerformAttack()
        {
            if (!CanAttack) return;
            LastAttackTime = Time.time;

            Invoke(nameof(InvokeAttackFinished), weaponData.slowTimeWindow / weaponData.attackRate / AttackSpeed);
            
            if (IsPlayer) PlayerComponent.MovementAttackFactor = weaponData.movementFactor; //TODO enable for enemy
            
            if (WeaponAnimator) WeaponAnimator.SetTrigger("attack");
            
            AttackStarted?.Invoke(this, EventArgs.Empty);

            if (HasRangedWeapon) PerformRangedAttack();
            else if (HasMeleeWeapon) PerformMeleeAttack();
        }

        /// <summary>
        /// Performs a ranged attack with the equipped ranged weapon.
        /// </summary>
        void PerformRangedAttack()
        {
            if (!HasRangedWeapon) return;

            FireProjectile();
        }

        void FireProjectile()
        {
            var projectileObject = InstantiateProjectile;

            var projectileComponent = projectileObject.GetComponent<ProjectileComponent>();
            if (projectileComponent) projectileComponent.Fire(
                ParentRigidbody.velocity, 
                IsPlayer ? LoadoutComponent.CurrentLoadoutSlot.weaponCharges : 0, 
                DamageBonus, 
                DamageFactor, 
                AttackSpeed);
        }

        void PerformMeleeAttack()
        {
            if (!HasMeleeWeapon) return;
        }

        void OnAttackFinished(object sender, EventArgs e)
        {
            if (IsPlayer)
            {
                PlayerComponent.MovementAttackFactor = 1f;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!HasMeleeWeapon) return;
            
            var healthComponent = other.GetComponent<HealthComponent>();
            if (!healthComponent) return;

            healthComponent.Health -= (int)(MeleeWeaponData.damage * DamageFactor) + DamageBonus;
        }

        void InvokeAttackFinished() => AttackFinished?.Invoke(this, EventArgs.Empty);

        public void Consume(AttackItemData attackItem)
        {
            AttackSpeed = attackItem.attackSpeed;
            DamageFactor = attackItem.damageFactor;
            DamageBonus = attackItem.damageBonus;
            Invoke(nameof(ResetBuffs), attackItem.duration);
        }

        void ResetBuffs()
        {
            (AttackSpeed, DamageFactor, DamageBonus) = (1, 1, 0);

            if (ParticleSystem)
            {
                var mainModule = ParticleSystem.main;
                mainModule.simulationSpeed = _particleSimulationSpeed;
            }
        }
    }
}

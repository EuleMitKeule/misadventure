using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using HotlineHyrule.Entities;
using HotlineHyrule.Extensions;
using HotlineHyrule.Items;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace HotlineHyrule.Weapons
{
    /// <summary>
    /// Handles the behavior of the weapon it's attached to.
    /// </summary>
    public class WeaponComponent : MonoBehaviour
    {
        /// <summary>
        /// The position offset of the weapon object.
        /// </summary>
        [SerializeField] Vector3 weaponOffset;
        /// <summary>
        /// The currently equipped weapon.
        /// </summary>
        [SerializeField] public WeaponData weaponData;
        /// <summary>
        /// The input action to perform an attack.
        /// </summary>
        [SerializeField] public InputAction attackAction;

        float _attackSpeedFactor;
        float _particleSimulationSpeed;
        /// <summary>
        /// Multiplies the attack interval.
        /// </summary>
        float AttackSpeedFactor
        {
            get => _attackSpeedFactor;
            set
            {
                _attackSpeedFactor = value;
                if (WeaponAnimator) WeaponAnimator.SetFloat("attackSpeed", _attackSpeedFactor);
                if (ParticleSystem)
                {
                    var mainModule = ParticleSystem.main;
                    _particleSimulationSpeed = mainModule.simulationSpeed;
                    mainModule.simulationSpeed *= AttackSpeedFactor;
                }
            }
        }
        /// <summary>
        /// Multiplies the amount of damage the weapon deals.
        /// </summary>
        float DamageFactor { get; set; }
        /// <summary>
        /// Adds to the amount of damage the weapon deals.
        /// </summary>
        int DamageBonus { get; set; }
        /// <summary>
        /// The last point in time the weapon was used at.
        /// </summary>
        float LastAttackTime { get; set; }
        public bool CanMeleeAttack { get; set; }
        /// <summary>
        /// The currently equipped weapon object.
        /// </summary>
        GameObject WeaponObject { get; set; }

        /// <summary>
        /// Whether the weapon component's parent is the player.
        /// </summary>
        bool IsPlayer => PlayerComponent;
        /// <summary>
        /// Whether the attack input is currently being registered.
        /// </summary>
        bool IsRequestingAttack => attackAction.ReadValue<float>() != 0f;
        public float AttackDelay => 1 / weaponData.attackRate / AttackSpeedFactor;
        /// <summary>
        /// Whether enough time has passed since the last usage for the weapon to be used again.
        /// </summary>
        public bool CanAttack =>
            weaponData && (weaponData.attackRate == 0 || Time.time >= LastAttackTime + AttackDelay);

        /// <summary>
        /// The currently equipped weapon object's transform.
        /// </summary>
        Transform WeaponTransform => WeaponObject.transform;
        /// <summary>
        /// Whether the current weapon is a ranged one.
        /// </summary>
        public bool HasRangedWeapon => weaponData is RangedWeaponData;
        public bool HasTargetingWeapon => weaponData is TargetingWeaponData;
        /// <summary>
        /// Whether the current weapon is a melee one.
        /// </summary>
        public bool HasMeleeWeapon => weaponData is MeleeWeaponData;
        /// <summary>
        /// Whether the current weapon is a conjuring one.
        /// </summary>
        public bool HasConjuringWeapon => weaponData is ConjuringWeaponData;
        /// <summary>
        /// The currently equipped ranged weapon.
        /// </summary>
        public RangedWeaponData RangedWeaponData => weaponData as RangedWeaponData;
        /// <summary>
        /// The currently equipped melee weapon.
        /// </summary>
        MeleeWeaponData MeleeWeaponData => weaponData as MeleeWeaponData;
        /// <summary>
        /// The currently equipped conjuring weapon.
        /// </summary>
        ConjuringWeaponData ConjuringWeaponData => weaponData as ConjuringWeaponData;
        public TargetingWeaponData TargetingWeaponData => weaponData as TargetingWeaponData;

        /// <summary>
        /// The weapon object's current world position
        /// </summary>
        public Vector3 WeaponPosition => WeaponTransform.position;
        /// <summary>
        /// The offset of the projectile spawn position relative to the weapon position.
        /// </summary>
        Vector3 ProjectileSpawnOffset =>
            WeaponTransform.right * RangedWeaponData.spawnPosition.x +
            WeaponTransform.up * RangedWeaponData.spawnPosition.y;
        /// <summary>
        /// The spawn position of the projectile.
        /// </summary>
        Vector3 RangedProjectileSpawnPosition => ProjectileSpawnOffset + WeaponTransform.position;
        
        int ConjuredEntities { get; set; }
        SpawnerComponent SpawnerComponent { get; set; }

        /// <summary>
        /// Invoked when an attack is performed.
        /// </summary>
        public event EventHandler<WeaponEventArgs> AttackStarted;
        /// <summary>
        /// Invoked when an attack is finished.
        /// </summary>
        public event EventHandler<EventArgs> AttackFinished;

        SpriteRenderer SpriteRenderer { get; set; }
        Animator WeaponAnimator { get; set; }
        Rigidbody2D Rigidbody { get; set; }
        PlayerComponent PlayerComponent { get; set; }
        ParticleSystem ParticleSystem { get; set; }
        LoadoutComponent LoadoutComponent { get; set; }

        void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
            PlayerComponent = GetComponent<PlayerComponent>();
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
            LoadoutComponent = GetComponent<LoadoutComponent>();

            AttackFinished += OnAttackFinished;

            if (weaponData) SetWeapon(weaponData);
            ResetBuffs();
            
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
            if (WeaponObject && HasConjuringWeapon)
            {
                var spawnerComponent = WeaponObject.GetComponent<SpawnerComponent>();
                if (!spawnerComponent) return;
                ConjuredEntities = spawnerComponent.CurrentEntities;
            }

            if (WeaponObject) Destroy(WeaponObject);
            
            weaponData = newWeaponData;

            if (!weaponData) return;

            WeaponObject = Instantiate(weaponData.weaponPrefab, transform);
            WeaponTransform.localPosition = weaponOffset;
            WeaponAnimator = WeaponObject.GetComponent<Animator>();

            if (HasConjuringWeapon)
            {
                SpawnerComponent = WeaponObject.GetComponent<SpawnerComponent>();
                if (!SpawnerComponent) return;

                SpawnerComponent.CurrentEntities = ConjuredEntities;

                var spawnTilemapObject = GameObject.Find(ConjuringWeaponData.spawnTilemapName);
                if (!spawnTilemapObject) return;

                var spawnTilemap = spawnTilemapObject.GetComponent<Tilemap>();
                if (!spawnTilemap) return;

                SpawnerComponent.spawnTilemap = spawnTilemap;

            }
        }

        /// <summary>
        /// Changes the weapon object's absolute rotation.
        /// </summary>
        /// <param name="angle">The angle to set the rotation to.</param>
        public void SetWeaponRotation(float angle)
        {
            WeaponTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        /// <summary>
        /// Performs an attack if possible.
        /// </summary>
        public void PerformAttack()
        {
            if (!weaponData) return;
            if (!CanAttack) return;

            LastAttackTime = Time.time;

            if (IsPlayer) PlayerComponent.MovementAttackFactor = weaponData.movementFactor; //TODO enable for enemy
            
            if (WeaponAnimator) WeaponAnimator.SetTrigger("attack");
            
            var time = WeaponAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            Invoke(nameof(InvokeAttackFinished), time);

            var weaponEventArgs = new WeaponEventArgs(weaponData);
            AttackStarted?.Invoke(this, weaponEventArgs);
        }

        /// <summary>
        /// Performs a ranged attack with the equipped ranged weapon.
        /// </summary>
        public void PerformRangedAttack()
        {
            if (!HasRangedWeapon) return;

            StartCoroutine(PerformRangedAttackRoutine());
        }

        IEnumerator PerformRangedAttackRoutine()
        {
            if (!HasRangedWeapon) yield break;

            var angleDifference = RangedWeaponData.projectileAngle / RangedWeaponData.projectileCount;
            var startAngle = RangedWeaponData.projectileAngleOffset - RangedWeaponData.projectileAngle / 2 + angleDifference / 2;
            var projectileCount = RangedWeaponData.projectileCount;

            for (var i = 0; i < projectileCount; i++)
            {
                var angle = startAngle + i * angleDifference;
                if (RangedWeaponData.flip) angle *= -1f;
                var absoluteAngle = WeaponTransform.eulerAngles.z + angle;
                var direction = Vector3.up.RotateAroundZ(absoluteAngle);
            
                FireProjectile(direction);

                yield return new WaitForSeconds(RangedWeaponData.projectileDelay);
            }
        }

        /// <summary>
        /// Fires a new instance of the projectile associated with the equipped ranged weapon.
        /// </summary>
        void FireProjectile(Vector3 direction)
        {
            var fireAngle = Vector3.SignedAngle(Vector3.up, direction, Vector3.forward);
            var projectileObject = Instantiate(RangedWeaponData.projectilePrefab, RangedProjectileSpawnPosition,
                Quaternion.Euler(0f, 0f, fireAngle));

            var projectileComponent = projectileObject.GetComponent<ProjectileComponent>();
            if (projectileComponent) projectileComponent.Fire(
                direction,
                Rigidbody.velocity, 
                IsPlayer ? LoadoutComponent.CurrentLoadoutSlot.weaponCharges : 0, 
                DamageBonus, 
                DamageFactor, 
                AttackSpeedFactor);
            
            Locator.SoundComponent.PlaySound(RangedWeaponData.weaponFiredSound);
        }
        
        public void PerformTargetingAttack()
        {
            if (!HasTargetingWeapon) return;

            StartCoroutine(PerformTargetingAttackRoutine());
        }
        
        IEnumerator PerformTargetingAttackRoutine()
        {
            if (!HasTargetingWeapon) yield break;

            var playerPos = Locator.PlayerComponent.transform.position;
            
            for (var i = 0; i < TargetingWeaponData.spawnRings.Count; i++)
            {
                var spawnRing = TargetingWeaponData.spawnRings[i];
                var nProjectiles = spawnRing.numberOfProjectiles;
                for (var j = 0; j < nProjectiles; j++)
                {
                    var angle = (float) j / nProjectiles * 360f;
                    SpawnTargetingProjectile(playerPos, angle, spawnRing.radius);
                }

                Locator.SoundComponent.PlaySound(TargetingWeaponData.weaponFiredSound);
                yield return new WaitForSeconds(TargetingWeaponData.delayBetweenSpawnRings);
            }
        }

        void SpawnTargetingProjectile(Vector3 targetPos, float angle, float distance)
        {
            var rotation = Quaternion.Euler(0f, 0f, angle);
            var projectileObject = Instantiate(TargetingWeaponData.projectilePrefab, targetPos, rotation);
            projectileObject.transform.position += projectileObject.transform.right * distance;
            projectileObject.GetComponent<Rigidbody2D>().simulated = true;

            var projectileComponent = projectileObject.GetComponent<ProjectileComponent>();
            if (projectileComponent) projectileComponent.Fire(
                new Vector3(0f ,0f, angle),
                Rigidbody.velocity, 
                IsPlayer ? LoadoutComponent.CurrentLoadoutSlot.weaponCharges : 0, 
                DamageBonus, 
                DamageFactor, 
                AttackSpeedFactor);
        }

        /// <summary>
        /// Performs a melee attack with the equpped melee weapon.
        /// </summary>
        public void PerformMeleeAttack()
        {
            if (!HasMeleeWeapon) return;

            CanMeleeAttack = true;
        }

        public void PerformConjuringAttack()
        {
            if (!HasConjuringWeapon) return;

            var spawnerComponent = WeaponObject.GetComponent<SpawnerComponent>();
            if (!spawnerComponent) return;

            var maxOffset = (int)(ConjuringWeaponData.conjuringAmount * ConjuringWeaponData.conjuringAmountOffset);
            var offset = Random.Range(-maxOffset, maxOffset + 1);
            var amount = ConjuringWeaponData.conjuringAmount + offset;
            spawnerComponent.SpawnAround(transform.position, ConjuringWeaponData.conjuringRadius, amount);
        }

        void OnAttackFinished(object sender, EventArgs e)
        {
            if (IsPlayer)
            {
                PlayerComponent.MovementAttackFactor = 1f;
            }
        }

        // void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (!HasMeleeWeapon) return;
        //
        //     var healthComponent = other.GetComponent<HealthComponent>();
        //     if (!healthComponent) return;
        //
        //     healthComponent.Health -= (int)(MeleeWeaponData.damage * DamageFactor) + DamageBonus;
        // }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!CanMeleeAttack) return;
            if (!HasMeleeWeapon) return;

            var healthComponent = other.GetComponent<HealthComponent>();
            if (!healthComponent) return;

            healthComponent.Health -= (int) (MeleeWeaponData.damage * DamageFactor) + DamageBonus;
            CanMeleeAttack = false;
        }

        void InvokeAttackFinished() => AttackFinished?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Applies item effects of a given attack item.
        /// </summary>
        /// <param name="attackItem">The attack item to consume.</param>
        public void Consume(AttackItemData attackItem)
        {
            AttackSpeedFactor = attackItem.attackSpeed;
            DamageFactor = attackItem.damageFactor;
            DamageBonus = attackItem.damageBonus;
            Invoke(nameof(ResetBuffs), attackItem.duration);
        }

        /// <summary>
        /// Resets all currently applied attack item buffs.
        /// </summary>
        void ResetBuffs()
        {
            (AttackSpeedFactor, DamageFactor, DamageBonus) = (1, 1, 0);

            if (ParticleSystem)
            {
                var mainModule = ParticleSystem.main;
                mainModule.simulationSpeed = _particleSimulationSpeed;
            }
        }

        public void RegisterConjuringCallback(HealthComponent healthComponent)
        {
            healthComponent.HealthChanged += (sender, e) =>
            {
                if (!e.IsKilled) return;
                if (!SpawnerComponent)
                {
                    ConjuredEntities -= 1;
                }
                else
                {
                    SpawnerComponent.CurrentEntities -= 1;
                }                 
            };
        }

       
    }
}

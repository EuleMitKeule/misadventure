using System;
using HotlineHyrule.Entities;
using HotlineHyrule.Weapons.Projectiles;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    public class ProjectileComponent : MonoBehaviour
    {
        [SerializeField] RangedWeaponData rangedWeaponData;
        [SerializeField] ProjectileData projectileData;

        /// <summary>
        /// The distance between center and top of projectile.
        /// </summary>
        float TopOffset => Collider.bounds.extents.y;
        /// <summary>
        /// The start point of the impact raycast.
        /// </summary>
        Vector2 ImpactRaycastOrigin => Transform.position + Transform.up * projectileData.impactRaycastOffset;
        /// <summary>
        /// The length of the impact raycast.
        /// </summary>
        float ImpactRaycastDistance => TopOffset - projectileData.impactRaycastOffset;
        /// <summary>
        /// The result of the impact raycast.
        /// </summary>
        RaycastHit2D ImpactRaycastHit =>
            Physics2D.Raycast(ImpactRaycastOrigin, Transform.up, ImpactRaycastDistance, projectileData.impactMask);
        int DamageBonus { get; set; }
        float DamageFactor { get; set; }
        LinearProjectileData LinearProjectileData => (LinearProjectileData)projectileData;
        CurvedProjectileData CurvedProjectileData => (CurvedProjectileData)projectileData;
        bool IsLinearProjectile => projectileData is LinearProjectileData;
        bool IsCurvedProjectile => projectileData is CurvedProjectileData;
        float StartSpeed =>
            (CurvedProjectileData.range + Mathf.Pow(Rigidbody.velocity.magnitude, 2f) * Rigidbody.drag * Mathf.Pow(CurvedProjectileData.flightTime * TIME_FACTOR, 2f)) /
            (CurvedProjectileData.flightTime * TIME_FACTOR);
        float StartDrag => 1 / (CurvedProjectileData.flightTime * TIME_FACTOR);
        const float TIME_FACTOR = 1 / 4.638f;

        int WeaponCharges { get; set; }
        int Penetrations { get; set; }

        Transform Transform { get; set; }
        SpriteRenderer SpriteRenderer { get; set; }
        Rigidbody2D Rigidbody { get; set; }
        Collider2D Collider { get; set; }
        Animator Animator { get; set; }
        ParticleSystem ParticleSystem { get; set; }

        void Awake()
        {
            Transform = GetComponent<Transform>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            Animator = GetComponent<Animator>();
            ParticleSystem = GetComponent<ParticleSystem>();
        }

        void Start()
        {
            var impactRaycastHit = ImpactRaycastHit;

            if (impactRaycastHit)
            {
                HandleImpact(impactRaycastHit.transform);

                Transform.position = impactRaycastHit.centroid;
            }
        }

        void FixedUpdate()
        {
            if (!IsCurvedProjectile) return;

            if (Rigidbody.velocity.magnitude < CurvedProjectileData.movementThreshold) HandleImpact();
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (projectileData.impactMask.value != (projectileData.impactMask.value | 1 << other.gameObject.layer)) return;

            HandleImpact(other.transform);

            var healthComponent = other.gameObject.GetComponentInParent<HealthComponent>();
            if (healthComponent) healthComponent.Health -= (int)(projectileData.damage * DamageFactor) + DamageBonus;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (projectileData.impactMask.value != (projectileData.impactMask.value | 1 << other.gameObject.layer)) return;

            if (Penetrations <= 0 || other.gameObject.layer.IsWall()) HandleImpact(other.transform);
            Penetrations -= 1;

            var healthComponent = other.GetComponentInParent<HealthComponent>();
            if (healthComponent) healthComponent.Health -= (int)(projectileData.damage * DamageFactor) + DamageBonus;
        }

        void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

        public void Fire(Vector2 entityVelocity, int weaponCharges, int damageBonus, float damageFactor, float attackSpeed)
        {
            if (projectileData is LinearProjectileData linearProjectileData)
            {
                var velocity = linearProjectileData.movementSpeed == 0f
                    ? entityVelocity
                    : (Vector2)Transform.up * linearProjectileData.movementSpeed;

                Rigidbody.velocity = velocity;
            }
            else if (projectileData is CurvedProjectileData)
            {
                Rigidbody.drag = StartDrag;
                Rigidbody.velocity = Transform.up * StartSpeed;
            }
            
            DamageBonus = damageBonus;
            DamageFactor = damageFactor;
            WeaponCharges = weaponCharges;
            Penetrations = projectileData.penetrations;

            if (Animator)
            {
                Animator.SetTrigger("attack");
                Animator.SetFloat("attackSpeed", attackSpeed);
            }

            if (ParticleSystem)
            {
                var mainModule = ParticleSystem.main;
                mainModule.simulationSpeed *= attackSpeed;
            }
        }

        /// <summary>
        /// Handles the influence of a collision on the projectile.
        /// </summary>
        void HandleImpact(Transform other = null)
        {
            Rigidbody.simulated = false;

            if (projectileData.isSticky) if (other) Transform.SetParent(other);

            if (Animator) Animator.SetTrigger("impact");
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void DropWeapon()
        {
            if (WeaponCharges <= 0) return;
            if (!rangedWeaponData.droppedWeaponPrefab) return;

            var droppedWeaponObject = Instantiate(rangedWeaponData.droppedWeaponPrefab, transform.position,
                transform.rotation);

            var droppedWeaponComponent = droppedWeaponObject.GetComponent<DroppedWeaponComponent>();
            if (droppedWeaponComponent) droppedWeaponComponent.weaponCharges = WeaponCharges;
        }
    }
}

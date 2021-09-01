using System;
using System.Collections.Generic;
using System.Linq;
using Misadventure.Entities;
using Misadventure.Items;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Misadventure.Weapons.Projectiles
{
    public class ProjectileComponent : MonoBehaviour
    {
        /// <summary>
        /// The ranged weapon associated with this projectile object.
        /// </summary>
        [SerializeField] public RangedWeaponData rangedWeaponData;
        /// <summary>
        /// The projectile associated with this projectile object.
        /// </summary>
        [SerializeField] public ProjectileData projectileData;
        
        /// <summary>
        /// If impact animation trigger shall be triggered when impact happens
        /// </summary>
        [SerializeField] bool useImpactAnimation = true;

        [SerializeField] bool stopPhysicsOnImpact = true;

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

        /// <summary>
        /// Adds to the absolute damage the projectile deals on impact.
        /// </summary>
        int DamageBonus { get; set; }
        /// <summary>
        /// Multiplies the absolute damage the projectile deals on impact.
        /// </summary>
        float DamageFactor { get; set; }
        /// <summary>
        /// The number of charges remaining on the ranged weapon after firing this projectile.
        /// </summary>
        int WeaponCharges { get; set; }
        /// <summary>
        /// The number of entity penetrations remaining until impact.
        /// </summary>
        int Penetrations { get; set; }
        List<GameObject> HitEnemies { get; set; } = new List<GameObject>();

        /// <summary>
        /// Whether the projectile associated with this projectile object is linear.
        /// </summary>
        bool IsLinearProjectile => projectileData is LinearProjectileData;
        /// <summary>
        /// Whether the projectile associated with this projectile object is curved.
        /// </summary>
        bool IsCurvedProjectile => projectileData is CurvedProjectileData;
        /// <summary>
        /// The linear projectile associated with this projectile object.
        /// </summary>
        LinearProjectileData LinearProjectileData => (LinearProjectileData)projectileData;
        /// <summary>
        /// The curved projectile associated with this projectile object.
        /// </summary>
        CurvedProjectileData CurvedProjectileData => (CurvedProjectileData)projectileData;

        /// <summary>
        /// The speed amount the curved projectile should start with.
        /// </summary>
        float CurvedStartSpeed =>
            (CurvedProjectileData.range + Mathf.Pow(Rigidbody.velocity.magnitude, 2f) *
            Rigidbody.drag * Mathf.Pow(CurvedProjectileData.flightTime * TIME_FACTOR, 2f)) /
            (CurvedProjectileData.flightTime * TIME_FACTOR);
        /// <summary>
        /// The linear drag amount the curved projectile should start with.
        /// </summary>
        float StartDrag => 1 / (CurvedProjectileData.flightTime * TIME_FACTOR);
        /// <summary>
        /// Scales drag time towards real time.
        /// </summary>
        const float TIME_FACTOR = 1 / 4.638f;

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

            if (Rigidbody.velocity.magnitude < CurvedProjectileData.movementThreshold) HandleStop();
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

        void OnParticleCollision(GameObject other)
        {
            if (projectileData.impactMask.value != (projectileData.impactMask.value | 1 << other.layer)) return;

            if (HitEnemies.Contains(other)) return;
            HitEnemies.Add((other));

            var healthComponent = other.GetComponentInParent<HealthComponent>();
            if (healthComponent) healthComponent.Health -= (int)(projectileData.damage * DamageFactor) + DamageBonus;
        }

        void OnBecameInvisible()
        {
            if (projectileData.destroyOnInvisible) Destroy(gameObject);
        }

        public void Fire(Vector3 direction, Vector2 entityVelocity, int weaponCharges, int damageBonus, float damageFactor, float attackSpeed)
        {
            if (projectileData is LinearProjectileData linearProjectileData)
            {
                var velocity = linearProjectileData.movementSpeed == 0f
                    ? entityVelocity
                    : (Vector2)direction * linearProjectileData.movementSpeed;

                Rigidbody.velocity = velocity;
            }
            else if (projectileData is CurvedProjectileData)
            {
                Rigidbody.drag = StartDrag;
                Rigidbody.velocity = direction * CurvedStartSpeed;
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
            if (stopPhysicsOnImpact) Rigidbody.simulated = false;

            var impactParticleSystem = projectileData.impactParticleSystem;
            if (impactParticleSystem) Instantiate(impactParticleSystem, transform.position, Quaternion.identity);
            if (Animator && useImpactAnimation) Animator.SetTrigger("impact");
            if (projectileData.isSticky) if (other) 
                if (other.gameObject.layer == LayerMask.NameToLayer("player") && projectileData.notStickyOnPlayer)
                    Destroy(gameObject);
                else
                    Transform.SetParent(other);
        }

        void HandleStop()
        {
            Rigidbody.simulated = false;

            if (Animator) Animator.SetTrigger("stop");
            var impactParticleSystem = projectileData.impactParticleSystem;
            if (impactParticleSystem) Instantiate(impactParticleSystem, transform.position, Quaternion.identity);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void DropWeapon()
        {
            if (!rangedWeaponData) return;
            if (!rangedWeaponData.hasInfiniteCharges && WeaponCharges <= 0) return;
            if (!rangedWeaponData.ItemPrefab) return;

            var droppedWeaponObject = Instantiate(rangedWeaponData.ItemPrefab, transform.position,
                transform.rotation);

            var droppedWeaponComponent = droppedWeaponObject.GetComponent<DroppedWeaponComponent>();
            if (droppedWeaponComponent) droppedWeaponComponent.weaponCharges = WeaponCharges;
        }

        public void SpawnEffectTiles(int count)
        {
            var weaponEffectTilemapObject = GameObject.Find(projectileData.weaponEffectTilemapName);
            if (!weaponEffectTilemapObject) return;

            var weaponEffectTilemap = weaponEffectTilemapObject.GetComponent<Tilemap>();
            if (!weaponEffectTilemap) return;

            var maxoffset = count * projectileData.weaponEffectRandomnes;
            var offset = Random.Range(-maxoffset, maxoffset);

            count += (int)offset;
            var spawnPositions = new List<Vector3Int>();

            for(int x = -projectileData.weaponEffectRadius; x <= projectileData.weaponEffectRadius; x++)
            {
                for(int y = -projectileData.weaponEffectRadius; y <= projectileData.weaponEffectRadius; y++)
                {
                    var distance = Mathf.Sqrt(x * x + y * y);
                    if (distance > projectileData.weaponEffectRadius) continue;
                    var cellPosition = weaponEffectTilemap.WorldToCell(transform.position);
                    var offsetPosition = new Vector3Int(x, y,0);

                    spawnPositions.Add(offsetPosition + cellPosition);
                }
            }

            count = Mathf.Clamp(count, 0, spawnPositions.Count);


            var effectTiles = new List<TileBase>();

            for (var i = 0; i < count; i++)
            {
                var rnd = Random.Range(0, projectileData.weaponEffectTiles.Count);
                var effectTile = projectileData.weaponEffectTiles[rnd];
                effectTiles.Add(effectTile);
            }

            spawnPositions = spawnPositions.OrderBy(e => Guid.NewGuid()).ToList();

            var tileEffectComponent = weaponEffectTilemap.GetComponent<TileEffectComponent>();
            if (!tileEffectComponent) return;
            for (var i = 0; i < count; i++)
            {
                var position = spawnPositions[i];
                tileEffectComponent.SetTile(position, effectTiles[i]);
            }
        }
    }
}

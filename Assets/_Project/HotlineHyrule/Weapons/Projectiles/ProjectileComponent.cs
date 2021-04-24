using System;
using HotlineHyrule.Entities;
using HotlineHyrule.Weapons.Projectiles;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    public class ProjectileComponent : MonoBehaviour
    {
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

        Transform Transform { get; set; }
        SpriteRenderer SpriteRenderer { get; set; }
        Rigidbody2D Rigidbody { get; set; }
        Collider2D Collider { get; set; }
        Animator Animator { get; set; }

        void Awake()
        {
            Transform = GetComponent<Transform>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            Animator = GetComponent<Animator>();
        }

        void Start()
        {
            if (!projectileData.isSticky)
            {
                var impactRaycastHit = ImpactRaycastHit;

                if (impactRaycastHit)
                {
                    HandleImpact(impactRaycastHit.transform);

                    Transform.position = impactRaycastHit.centroid;
                }
            }
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (projectileData.impactMask.value != (projectileData.impactMask.value | 1 << other.gameObject.layer)) return;
            
            HandleImpact(other.transform);

            var healthComponent = other.gameObject.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.Health -= (int)(projectileData.damage * DamageFactor) + DamageBonus;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (projectileData.impactMask.value != (projectileData.impactMask.value | 1 << other.gameObject.layer)) return;

            HandleImpact(other.transform);

            var healthComponent = other.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.Health -= (int)(projectileData.damage * DamageFactor) + DamageBonus;
        }

        void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

        public void Fire(Vector2 entityVelocity, int damageBonus, float damageFactor)
        {
            DamageBonus = damageBonus;
            DamageFactor = damageFactor;
            
            var velocity = projectileData.movementSpeed == 0f
                ? entityVelocity
                : (Vector2)Transform.up * projectileData.movementSpeed;

            Rigidbody.velocity = velocity;

            if (Animator) Animator.SetTrigger("attack");
        }

        /// <summary>
        /// Handles the influence of a collision on the projectile.
        /// </summary>
        void HandleImpact(Transform other)
        {
            if (!projectileData.isSticky)
            {
                if (projectileData.destroyOnImpact) Destroy(gameObject);
                return;
            }

            Rigidbody.velocity = Vector2.zero;
            Transform.SetParent(other);
            Rigidbody.simulated = false;
            if (SpriteRenderer) SpriteRenderer.sprite = projectileData.impactSprite;
        }
    }
}

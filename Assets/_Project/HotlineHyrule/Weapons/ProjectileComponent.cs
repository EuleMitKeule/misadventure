using System;
using HotlineHyrule.Entities;
using UnityEngine;

namespace HotlineHyrule.Weapons
{
    public class ProjectileComponent : MonoBehaviour
    {
        /// <summary>
        /// The sprite changed to after an impact.
        /// </summary>
        [SerializeField] Sprite impactSprite;
        /// <summary>
        /// Layermask that contains layers that count as impacts.
        /// </summary>
        [SerializeField] public LayerMask impactMask;
        /// <summary>
        /// Offsets the start point if the impact raycast.
        /// </summary>
        [SerializeField] float impactRaycastOffset;

        /// <summary>
        /// The distance between center and top of projectile.
        /// </summary>
        float TopOffset => Collider.bounds.extents.y;
        /// <summary>
        /// The start point of the impact raycast.
        /// </summary>
        Vector2 ImpactRaycastOrigin => Transform.position + Transform.up * impactRaycastOffset;
        /// <summary>
        /// The length of the impact raycast.
        /// </summary>
        float ImpactRaycastDistance => TopOffset - impactRaycastOffset;
        /// <summary>
        /// The result of the impact raycast.
        /// </summary>
        RaycastHit2D ImpactRaycastHit =>
            Physics2D.Raycast(ImpactRaycastOrigin, Transform.up, ImpactRaycastDistance, impactMask);

        public int ImpactDamage { get; set; }

        Transform Transform { get; set; }
        SpriteRenderer SpriteRenderer { get; set; }
        Rigidbody2D Rigidbody { get; set; }
        Collider2D Collider { get; set; }

        void Awake()
        {
            Transform = GetComponent<Transform>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
        }

        void Start()
        {
            var impactRaycastHit = ImpactRaycastHit;
            
            if (impactRaycastHit)
            {
                HandleCollision(impactRaycastHit.transform);

                Transform.position = impactRaycastHit.centroid;
            }
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (impactMask.value != (impactMask.value | 1 << other.gameObject.layer)) return;
            
            HandleCollision(other.transform);

            var healthComponent = other.gameObject.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.Health -= ImpactDamage;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (impactMask.value != (impactMask.value | 1 << other.gameObject.layer)) return;

            HandleCollision(other.transform);

            var healthComponent = other.GetComponent<HealthComponent>();
            if (healthComponent) healthComponent.Health -= ImpactDamage;
        }

        void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Handles the influence of a collision on the projectile.
        /// </summary>
        void HandleCollision(Transform other)
        {
            Rigidbody.velocity = Vector2.zero;
            Transform.SetParent(other);
            Rigidbody.simulated = false;
            if (SpriteRenderer) SpriteRenderer.sprite = impactSprite;
        }
    }
}

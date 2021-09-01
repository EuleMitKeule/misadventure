using Misadventure.Entities;
using Misadventure.Level;
using Misadventure.Weapons;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Misadventure.Items
{
    public class BombComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        ItemData BombCoverItem { get; set; }
        [OdinSerialize]
        Collider2D ExplosionCollider { get; set; }
        [OdinSerialize]
        Collider2D HitboxCollider { get; set; }
        [OdinSerialize]
        Collider2D SecondHitboxCollider { get; set; }
        [OdinSerialize]
        GameObject WallObject { get; set; }
        Animator Animator { get; set; }
        Light2D Light { get; set; }
        bool IsExploded { get; set; }
        bool CanExplode { get; set; }

        void Awake()
        {
            if (!ExplosionCollider) ExplosionCollider = GetComponents<Collider2D>()[0];
            if (!HitboxCollider) HitboxCollider = GetComponents<Collider2D>()[1];
            Animator = GetComponent<Animator>();
            Light = GetComponent<Light2D>();

            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (!Locator.PlayerComponent) return;
            var itemPickupComponent = Locator.PlayerComponent.GetComponent<ItemPickupComponent>();
            if (!itemPickupComponent) return;
            itemPickupComponent.ItemConsumed += OnItemConsumed;
        }

        void OnItemConsumed(object sender, ItemEventArgs e)
        {
            if (e.ItemData != BombCoverItem) return;

            CanExplode = true;
            HitboxCollider.enabled = true;
            if (SecondHitboxCollider) SecondHitboxCollider.enabled = true;
            Light.enabled = true;
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (!CanExplode) return;

            if (!IsExploded)
            {
                var isMeleeWeapon = other.gameObject.GetComponent<WeaponAnimationComponent>();

                if (!other.gameObject.layer.IsPlayerProjectile() & !isMeleeWeapon) return;
                HandleExplosion();
            }
        }

        void OnParticleCollision(GameObject other)
        {
            if (!CanExplode) return;

            if (!IsExploded)
            {
                var isMeleeWeapon = other.GetComponent<WeaponAnimationComponent>();

                if (!other.layer.IsPlayerProjectile() &&
                    other.layer != 20 & !other.layer.IsPlayer() & !isMeleeWeapon) return;
                HandleExplosion();
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!CanExplode) return;

            if (!IsExploded)
            {
                var isMeleeWeapon = other.GetComponent<WeaponAnimationComponent>();

                if (!other.gameObject.layer.IsPlayerProjectile() &! isMeleeWeapon) return;

                HandleExplosion();
                return;
            }

            var healthComponent = other.GetComponent<HealthComponent>();
            if (!healthComponent) return;
            healthComponent.Health -= healthComponent.maxHealth;
        }

        void HandleExplosion()
        {
            IsExploded = true;
            Animator.SetTrigger("explode");
            if (WallObject) Destroy(WallObject);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}

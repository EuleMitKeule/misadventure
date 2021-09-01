using HotlineHyrule.Entities;
using HotlineHyrule.Weapons;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Items
{
    public class DamComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        Tilemap DamTilemap { get; set; }

        void OnCollisionEnter2D(Collision2D other)
        {
            var isMeleeWeapon = other.gameObject.GetComponent<WeaponAnimationComponent>();

            if (!other.gameObject.layer.IsPlayerProjectile() & !isMeleeWeapon) return;

            HandleDestruction();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var isMeleeWeapon = other.GetComponent<WeaponAnimationComponent>();

            if (!other.gameObject.layer.IsPlayerProjectile() & !isMeleeWeapon) return;
            HandleDestruction();
        }

        void OnParticleCollision(GameObject other)
        {
            var isMeleeWeapon = other.GetComponent<WeaponAnimationComponent>();

            if (!other.layer.IsPlayerProjectile() && other.layer != 20 & !other.layer.IsPlayer() & !isMeleeWeapon) return;
            HandleDestruction();
        }

        void HandleDestruction()
        {
            var cellPosition = Locator.LevelComponent.GetComponent<Grid>().WorldToCell(transform.position);
            DamTilemap.SetTile(cellPosition, null);
            Destroy(gameObject);
        }
    }
}
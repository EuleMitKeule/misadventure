using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Weapons.Projectiles
{
    public class ProjectileData : ScriptableObject
    {
        /// <summary>
        /// The damage the projectile deals to entities on impact.
        /// </summary>
        [SerializeField] public int damage;
        /// <summary>
        /// Layers that count as impact.
        /// </summary>
        [SerializeField] public LayerMask impactMask;
        /// <summary>
        /// Offsets the start point of the impact raycast.
        /// </summary>
        [SerializeField] public float impactRaycastOffset;
        /// <summary>
        /// Whether the projectile should stick to entities on impact.
        /// </summary>
        [SerializeField] public bool isSticky;
        /// <summary>
        /// How often the projectile can penetrate entities.
        /// </summary>
        [SerializeField] public int penetrations;
        /// <summary>
        /// The particle system to spawn on impact.
        /// </summary>
        [SerializeField] public GameObject impactParticleSystem;
        /// <summary>
        /// Tiles to spawn when exploding.
        /// </summary>
        [SerializeField] public List<TileBase> weaponEffectTiles;
        /// <summary>
        /// The name of the tilemap to spawn the tiles on.
        /// </summary>
        [SerializeField] public string weaponEffectTilemapName = "tilemap_weapon_effect";
    }
}
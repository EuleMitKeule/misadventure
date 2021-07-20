using Sirenix.OdinInspector;
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
        /// Whether projectile should not stick on player even when impact mask for player and sticky are enabled
        /// </summary>
        [SerializeField] public bool notStickyOnPlayer;
        /// <summary>
        /// Whether the projectile gets destroyed when it's not rendered.
        /// </summary>
        [SerializeField]
        public bool destroyOnInvisible;
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
        [SerializeField][BoxGroup("Effect")] public List<TileBase> weaponEffectTiles;
        /// <summary>
        /// The name of the tilemap to spawn the tiles on.
        /// </summary>
        [SerializeField][BoxGroup("Effect")] public string weaponEffectTilemapName = "tilemap_weapon_effect";
        /// <summary>
        /// Spawnraduis
        /// </summary>
        [SerializeField][BoxGroup("Effect")] public int weaponEffectRadius;
        /// <summary>
        /// 
        /// </summary>
        [SerializeField][BoxGroup("Effect")][Range(0,1)] public float weaponEffectRandomnes;
    }
}
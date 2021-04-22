using UnityEngine;

namespace HotlineHyrule.Weapons.Projectiles
{
    [CreateAssetMenu(menuName = "Weapons/New Projectile Data")]
    public class ProjectileData : ScriptableObject
    {
        [SerializeField] public int damage;

        /// <summary>
        /// The speed the shot projectile travels at.
        /// </summary>
        [SerializeField] public float movementSpeed;

        /// <summary>
        /// The sprite changed to after an impact.
        /// </summary>
        [SerializeField] public Sprite impactSprite;

        /// <summary>
        /// Layermask that contains layers that count as impacts.
        /// </summary>
        [SerializeField] public LayerMask impactMask;

        /// <summary>
        /// Offsets the start point if the impact raycast.
        /// </summary>
        [SerializeField] public float impactRaycastOffset;

        /// <summary>
        /// Whether the projectile stick to the object they collide with.
        /// </summary>
        [SerializeField] public bool isSticky;
    }
}
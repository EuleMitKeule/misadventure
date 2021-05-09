using UnityEngine;

namespace HotlineHyrule.Weapons.Projectiles
{
    [CreateAssetMenu(menuName = "Weapon/New Curved Projectile")]
    public class CurvedProjectileData : ProjectileData
    {
        [SerializeField] public float range = 10f;
        [SerializeField] public float flightTime = 1f;
        [Range(0.01f, 0.5f)] [SerializeField] public float movementThreshold = 0.1f;
    }
}
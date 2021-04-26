using UnityEngine;

namespace HotlineHyrule.Weapons.Projectiles
{
    [CreateAssetMenu(menuName = "Weapon/New Curved Projectile")]
    public class CurvedProjectileData : ProjectileData
    {
        [SerializeField] public float range;
        [SerializeField] public float flightTime;
        [Range(0.01f, 0.5f)] [SerializeField] public float movementThreshold;
    }
}
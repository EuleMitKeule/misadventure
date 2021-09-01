using System;
using System.Collections.Generic;
using UnityEngine;

namespace Misadventure.Weapons
{
    [Serializable]
    public struct ProjectileSpawnRing
    {
        public int numberOfProjectiles;
        public float radius;
    }
    
    [CreateAssetMenu(menuName = "Weapon/New Targeting Weapon")]
    public class TargetingWeaponData : WeaponData
    {
        [SerializeField] public GameObject projectilePrefab;
        [SerializeField] public float delayBetweenSpawnRings;
        [SerializeField] public Vector2 offset;
        [SerializeField] public AudioClip weaponFiredSound;
        [SerializeField] public List<ProjectileSpawnRing> spawnRings;
    }
}
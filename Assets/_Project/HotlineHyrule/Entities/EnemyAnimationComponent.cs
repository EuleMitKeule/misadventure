using System;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    public class EnemyAnimationComponent : MonoBehaviour
    {
        [SerializeField] GameObject deathParticleSystemPrefab;

        public void SpawnDeathParticleSystem()
        {
            if (!deathParticleSystemPrefab) return;
            Instantiate(deathParticleSystemPrefab, transform.position, Quaternion.identity);
        }

        public void Destroy() => Destroy(gameObject);
    }
}
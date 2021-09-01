using Misadventure.Entities.EnemyStates;
using UnityEngine;

namespace Misadventure.Entities
{
    public class EnemyAnimationComponent : MonoBehaviour
    {
        /// <summary>
        /// The particle system to spawn when the enemy dies.
        /// </summary>
        [SerializeField] GameObject deathParticleSystemPrefab;

        EnemyComponent EnemyComponent { get; set; }

        void Awake()
        {
            EnemyComponent = GetComponent<EnemyComponent>();
        }

        public void SpawnDeathParticleSystem()
        {
            if (!deathParticleSystemPrefab) return;
            Instantiate(deathParticleSystemPrefab, transform.position, Quaternion.identity);
        }

        public void SetStateToFollow()
        {
            EnemyComponent.SetState<EnemyFollowStateComponent>();
        }

        public void Destroy() => Destroy(gameObject);

        public void LockRotation()
        {
            EnemyComponent.LockRotation = true;
        }

        public void UnlockRotation()
        {
            EnemyComponent.LockRotation = false;
        }

        public void LockVelocity()
        {
            EnemyComponent.LockVelocity = true;
        }
        
        public void UnlockVelocity()
        {
            EnemyComponent.LockVelocity = false;
        }
    }
}
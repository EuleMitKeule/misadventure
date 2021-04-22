using System.Collections;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateShootProjectileComponent : EnemyStateBaseComponent
    {
        /// <summary>
        /// Projectile prefab that shall be spawned
        /// </summary>
        [SerializeField] GameObject projectileObj;
        Animator _animator;
        
        public override void Setup()
        {
            base.Setup();
            _animator = GetComponent<Animator>();
            StartCoroutine(ShootProjectile());
        }

        IEnumerator ShootProjectile()
        {
            _animator.SetTrigger("enterShootProjectileState");
            yield return new WaitForSeconds(0.5f);
            var instance = Instantiate(projectileObj, transform.position, Quaternion.Euler(transform.eulerAngles));
            var projectileComponent = instance.GetComponent<ProjectileComponent>();
            if (projectileComponent) projectileComponent.Fire(Vector2.zero);
            yield return new WaitForSeconds(0.5f);
            _animator.SetTrigger("enterPatrolState");
            enemyComponent.ChangeState(enemyComponent.PatrolState);
        }
    }
}
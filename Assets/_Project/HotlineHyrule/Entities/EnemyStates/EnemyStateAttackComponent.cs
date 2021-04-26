using System.Collections;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateAttackComponent : EnemyStateBaseComponent
    {
        public override void Setup()
        {
            base.Setup();

            StartCoroutine(PerformAttackRoutine());
        }

        public override void FixedStateUpdate()
        {
            base.FixedStateUpdate();
            Rigidbody.velocity = Vector2.zero;
        }

        IEnumerator PerformAttackRoutine()
        {
            if (Animator) Animator.SetTrigger("attack");

            yield return new WaitForSeconds(0.5f);

            if (WeaponComponent) WeaponComponent.PerformAttack();

            // var instance = Instantiate(projectileObj, transform.position, Quaternion.Euler(transform.eulerAngles));
            // var projectileComponent = instance.GetComponent<ProjectileComponent>();
            // if (projectileComponent) projectileComponent.Fire(Vector2.zero);

            yield return new WaitForSeconds(0.5f);

            // _animator.SetTrigger("enterPatrolState");

            EnemyComponent.ChangeState(EnemyComponent.FollowState);
        }
    }
}
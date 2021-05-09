using System.Collections;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateAttackComponent : EnemyStateBaseComponent
    {
        Coroutine AttackCoroutine { get; set; }

        public override void Setup()
        {
            base.Setup();
            
            if (Animator) Animator.SetBool("isMoving", false);

            StartAttackRoutine();
        }

        public override void Exit()
        {
            base.Exit();

            StopAttackRoutine();
        }

        public override void FixedStateUpdate()
        {
            base.FixedStateUpdate();

            Rigidbody.velocity = Vector2.zero;
            transform.rotation = EnemyComponent.FollowRotation;

            if (!EnemyComponent.IsPlayerAttackable)
            {
                StopAttackRoutine();
                if (EnemyComponent.IsPlayerVisible)
                {
                    EnemyComponent.ChangeState(EnemyComponent.FollowState);
                }
                else
                {
                    EnemyComponent.ChangeState(EnemyComponent.SearchState ? EnemyComponent.SearchState : EnemyComponent.PatrolState);
                }
            }
        }

        void StartAttackRoutine()
        {
            AttackCoroutine ??= StartCoroutine(AttackRoutine());
        }

        void StopAttackRoutine()
        {
            if (AttackCoroutine == null) return;
            
            StopCoroutine(AttackCoroutine);
            AttackCoroutine = null;
        }

        IEnumerator AttackRoutine()
        {
            while (true)
            {
                if (WeaponComponent.CanAttack)
                {
                    if (Animator) Animator.SetTrigger("attack");
                    if (WeaponComponent) WeaponComponent.PerformAttack();   
                }

                yield return new WaitForSeconds(WeaponComponent.AttackDelay);

            }
        }
    }
}
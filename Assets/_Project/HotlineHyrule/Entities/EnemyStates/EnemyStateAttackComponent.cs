using System.Collections;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateAttackComponent : EnemyStateBaseComponent
    {
        Coroutine AttackCoroutine { get; set; }

        public override void EnterState()
        {
            base.EnterState();

            StartAttackRoutine();
        }

        public override void ExitState()
        {
            base.ExitState();

            StopAttackRoutine();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();

            EnemyComponent.SetVelocity(Vector2.zero);
            transform.rotation = EnemyComponent.FollowRotation;

            if (!EnemyComponent.IsPlayerAttackable)
            {
                StopAttackRoutine();

                if (EnemyComponent.IsPlayerFollowable)
                {
                    EnemyComponent.ChangeState(EnemyComponent.FollowState);
                }
                else
                {
                    EnemyComponent.ChangeState(EnemyComponent.SearchState ? EnemyComponent.SearchState : EnemyComponent.PatrolState);
                }
            }
        }

        void StartAttackRoutine() => AttackCoroutine ??= StartCoroutine(AttackRoutine());

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
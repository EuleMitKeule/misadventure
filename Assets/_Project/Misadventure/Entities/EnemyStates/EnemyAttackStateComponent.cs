using System.Collections;
using UnityEngine;

namespace Misadventure.Entities.EnemyStates
{
    public class EnemyAttackStateComponent : EnemyBaseStateComponent
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
                    SetState<EnemyFollowStateComponent>();
                }
                else
                {
                    SetState(AfterAttackState);
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

        protected virtual IEnumerator AttackRoutine()
        {
            while (true)
            {
                if (WeaponComponent.CanAttack)
                {
                    if (Animator) Animator.SetTrigger(EnemyComponent.AttackAnimationTrigger);
                    if (WeaponComponent) WeaponComponent.PerformAttack();   
                }

                yield return new WaitForSeconds(WeaponComponent.AttackDelay);
            }
        }
    }
}
using System.Collections;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateAttackComponent : EnemyStateBaseComponent
    {
        /// <summary>
        /// Delay (in seconds) for performing the attack.
        /// Exmaple usuage is to fit the attack to the animation
        /// </summary>
        [SerializeField] float performAttackDelay = 0.5f;
        /// <summary>
        /// Delay when the attack state shall be left after the attack got performed
        /// </summary>
        [SerializeField] float changeStateDelay = 0.5f;

        Coroutine AttackCoroutine { get; set; }

        public override void Setup()
        {
            base.Setup();

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
                if (Animator) Animator.SetTrigger("attack");

                yield return new WaitForSeconds(performAttackDelay);

                if (WeaponComponent) WeaponComponent.PerformAttack();
            }
        }
    }
}
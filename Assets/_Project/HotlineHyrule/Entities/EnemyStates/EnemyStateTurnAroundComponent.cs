using System.Collections;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateTurnAroundComponent : EnemyStateBaseComponent
    {
        [SerializeField] float turnDelay;

        Coroutine TurnAroundCoroutine { get; set; }

        public override void EnterState()
        {
            base.EnterState();
            
            if (Animator) Animator.SetBool("isMoving", false);

            StartTurnAroundRoutine();
        }

        public override void ExitState()
        {
            base.ExitState();

            StopTurnAroundCoroutine();
        }

        public override void StateFixedUpdate()
        {
            base.StateFixedUpdate();
            
            Rigidbody.velocity = Vector2.zero;
            
            if (EnemyComponent.IsPlayerAttackable) EnemyComponent.ChangeState(EnemyComponent.AttackState);
            if (EnemyComponent.IsPlayerFollowable) EnemyComponent.ChangeState(EnemyComponent.FollowState);
        }

        void StartTurnAroundRoutine()
        {
            if (TurnAroundCoroutine != null) EnemyComponent.ChangeState(EnemyComponent.PatrolState);

            TurnAroundCoroutine ??= StartCoroutine(TurnAroundRoutine());
        }

        IEnumerator TurnAroundRoutine()
        {
            yield return new WaitForSeconds(turnDelay);

            transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + 90f);
            
            yield return new WaitForSeconds(turnDelay);

            transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z - 180f);

            yield return new WaitForSeconds(turnDelay);

            transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + 90f);

            TurnAroundCoroutine = null;
            EnemyComponent.ChangeState(EnemyComponent.PatrolState);
        }

        void StopTurnAroundCoroutine()
        {
            if (TurnAroundCoroutine == null) return;
            
            StopCoroutine(TurnAroundCoroutine);
            TurnAroundCoroutine = null;
        }
    }
}
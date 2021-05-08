using System.Collections;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateTurnAroundComponent : EnemyStateBaseComponent
    {
        [SerializeField] float turnDelay;

        Coroutine TurnAroundCoroutine { get; set; }

        public override void Setup()
        {
            base.Setup();

            StartTurnAroundRoutine();
        }

        public override void Exit()
        {
            base.Exit();

            StopTurnAroundCoroutine();
        }

        public override void FixedStateUpdate()
        {
            base.FixedStateUpdate();
            
            Rigidbody.velocity = Vector2.zero;
            
            if (EnemyComponent.IsPlayerAttackable) EnemyComponent.ChangeState(EnemyComponent.AttackState);
            if (EnemyComponent.IsPlayerVisible) EnemyComponent.ChangeState(EnemyComponent.FollowState);
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
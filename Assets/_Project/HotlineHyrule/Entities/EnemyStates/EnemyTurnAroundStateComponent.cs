using System.Collections;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyTurnAroundStateComponent : EnemyBaseStateComponent
    {
        [SerializeField] float turnDelay;

        Coroutine TurnAroundCoroutine { get; set; }

        public override void EnterState()
        {
            base.EnterState();

            StartTurnAroundRoutine();
        }

        public override void ExitState()
        {
            base.ExitState();

            StopTurnAroundCoroutine();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            
            EnemyComponent.SetVelocity(Vector2.zero);
            
            if (EnemyComponent.IsPlayerAttackable) EnemyComponent.ChangeState(EnemyComponent.AttackState);
            if (EnemyComponent.IsPlayerFollowable) EnemyComponent.ChangeState(EnemyComponent.FollowState);
        }

        void StartTurnAroundRoutine()
        {
            var passiveState = EnemyComponent.PatrolState
                ? EnemyComponent.PatrolState
                : EnemyComponent.GuardState;
            if (TurnAroundCoroutine != null) EnemyComponent.ChangeState(passiveState);

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
            var passiveState = EnemyComponent.PatrolState
                ? EnemyComponent.PatrolState
                : EnemyComponent.GuardState;
            EnemyComponent.ChangeState(passiveState);
        }

        void StopTurnAroundCoroutine()
        {
            if (TurnAroundCoroutine == null) return;
            
            StopCoroutine(TurnAroundCoroutine);
            TurnAroundCoroutine = null;
        }

        public override void OnHealthChanged(object sender, HealthEventArgs e)
        {
            transform.rotation = EnemyComponent.FollowRotation;
        }
    }
}
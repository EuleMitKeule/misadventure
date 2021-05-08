using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateSearchComponent : EnemyStateBaseComponent
    {
        [SerializeField] float followSpeed;

        public Vector3 LastSeenPosition { get; set; }

        public override void Setup()
        {
            base.Setup();

            LastSeenPosition = EnemyComponent.PlayerPosition;
            PathfindingComponent.SetDestination(Locator.LevelComponent.Grid.WorldToCell(LastSeenPosition));
        }

        public override void FixedStateUpdate()
        {
            Rigidbody.velocity = PathfindingComponent.CurrentDirection * followSpeed;
            // transform.LookAt(PathfindingComponent.CurrentDirection);

            if (!PathfindingComponent.hasWaypoint)
            {
                EnemyComponent.ChangeState(EnemyComponent.TurnAroundState);
            }

            if (EnemyComponent.IsPlayerInAttackRange)
            {
                EnemyComponent.ChangeState(EnemyComponent.AttackState);
            }

            if (EnemyComponent.IsPlayerInFollowRange)
            {
                EnemyComponent.ChangeState(EnemyComponent.FollowState);
            }
        }
    }
}
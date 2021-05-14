using System;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateSearchComponent : EnemyStateBaseComponent
    {
        /// <summary>
        /// The movement speed of the enemy while searching.
        /// </summary>
        [SerializeField] float moveSpeed;

        public Vector3 LastSeenPosition { get; set; }

        public override void EnterState()
        {
            base.EnterState();

            LastSeenPosition = EnemyComponent.PlayerPosition;
            PathfindingComponent.SetDestination(Locator.LevelComponent.Grid.WorldToCell(LastSeenPosition));

            PathfindingComponent.DestinationReached += OnDestinationReached;
        }

        public override void ExitState()
        {
            base.ExitState();
            
            PathfindingComponent.ClearDestination();
        }

        public override void UpdateState()
        {
            base.UpdateState();

#if UNITY_EDITOR
            Debug.DrawLine(transform.position, LastSeenPosition, Color.yellow);
#endif
        }

        public override void FixedUpdateState()
        {
            EnemyComponent.SetVelocity(PathfindingComponent.CurrentDirection * moveSpeed);
            if (Rigidbody.velocity != Vector2.zero) transform.rotation = EnemyComponent.WalkRotation;

            if (EnemyComponent.IsPlayerAttackable)
            {
                EnemyComponent.ChangeState(EnemyComponent.AttackState);
            }

            if (EnemyComponent.IsPlayerFollowable)
            {
                EnemyComponent.ChangeState(EnemyComponent.FollowState);
            }
        }

        void OnDestinationReached(object sender, EventArgs e)
        {
            EnemyComponent.ChangeState(EnemyComponent.TurnAroundState);
        }

        public override void OnHealthChanged(object sender, HealthEventArgs e)
        {
            transform.rotation = EnemyComponent.FollowRotation;
        }
    }
}
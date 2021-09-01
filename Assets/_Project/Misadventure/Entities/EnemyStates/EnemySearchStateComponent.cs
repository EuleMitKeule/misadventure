using System;
using UnityEngine;

namespace Misadventure.Entities.EnemyStates
{
    public class EnemySearchStateComponent : EnemyBaseStateComponent
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

            if (PathfindingComponent) PathfindingComponent.DestinationReached += OnDestinationReached;
        }

        public override void ExitState()
        {
            base.ExitState();

            if (PathfindingComponent) PathfindingComponent.DestinationReached -= OnDestinationReached;
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
                SetState<EnemyAttackStateComponent>();
            }

            if (EnemyComponent.IsPlayerFollowable)
            {
                SetState<EnemyFollowStateComponent>();
            }
        }

        void OnDestinationReached(object sender, EventArgs e)
        {
            SetState<EnemyTurnAroundStateComponent>();
        }

        public override void OnHealthChanged(object sender, HealthEventArgs e)
        {
            transform.rotation = EnemyComponent.FollowRotation;
        }
    }
}
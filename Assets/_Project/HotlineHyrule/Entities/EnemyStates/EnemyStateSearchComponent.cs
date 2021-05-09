using System;
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
            
            if (Animator) Animator.SetBool("isMoving", true);

            LastSeenPosition = EnemyComponent.PlayerPosition;
            PathfindingComponent.SetDestination(Locator.LevelComponent.Grid.WorldToCell(LastSeenPosition));

            PathfindingComponent.DestinationReached += OnDestinationReached;
        }

        public override void Exit()
        {
            base.Exit();
            
            PathfindingComponent.ClearDestination();
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            
            Debug.DrawLine(transform.position, LastSeenPosition, Color.yellow);
        }

        public override void FixedStateUpdate()
        {
            Rigidbody.velocity = PathfindingComponent.CurrentDirection * followSpeed;
            if (Rigidbody.velocity != Vector2.zero) transform.rotation = EnemyComponent.WalkRotation;

            if (EnemyComponent.IsPlayerAttackable)
            {
                EnemyComponent.ChangeState(EnemyComponent.AttackState);
            }

            if (EnemyComponent.IsPlayerVisible)
            {
                EnemyComponent.ChangeState(EnemyComponent.FollowState);
            }
        }

        void OnDestinationReached(object sender, EventArgs e)
        {
            EnemyComponent.ChangeState(EnemyComponent.TurnAroundState);
        }
    }
}
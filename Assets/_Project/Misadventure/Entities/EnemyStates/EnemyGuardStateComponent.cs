using Misadventure.Extensions;
using Misadventure.Level;
using Misadventure.Pathfinding;
using UnityEngine;

namespace Misadventure.Entities.EnemyStates
{
    public class EnemyGuardStateComponent : EnemyBaseStateComponent
    {
        [SerializeField] float moveSpeed;

        Vector3Int GuardPosition { get; set; }
        float GuardRotation { get; set; }
        bool IsAtGuardPosition { get; set; } = true;

        protected override void Awake()
        {
            base.Awake();

            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (!Locator.LevelComponent) return;
            var grid = Locator.LevelComponent.Grid;

            GuardPosition = grid.WorldToCell(transform.position);
            GuardRotation = transform.eulerAngles.z;
            
            if (!PathfindingComponent) return;
            PathfindingComponent.SetDestination(GuardPosition);
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
        }
        void OnDestinationReached(object sender, CellEventArgs e)
        {
            IsAtGuardPosition = true;
            transform.rotation = Quaternion.Euler(0f, 0f, GuardRotation);
        }

        public override void EnterState()
        {
            base.EnterState();

            if (PathfindingComponent) PathfindingComponent.DestinationReached += OnDestinationReached;

            IsAtGuardPosition = GuardPosition.ToWorld().DistanceTo(transform.position) <= 1f;

            if (!PathfindingComponent) return;
            PathfindingComponent.SetDestination(GuardPosition);
        }

        public override void ExitState()
        {
            base.ExitState();
            
            if (PathfindingComponent) PathfindingComponent.DestinationReached -= OnDestinationReached;
            PathfindingComponent.ClearDestination();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();

            if (EnemyComponent.IsPlayerAttackable)
            {
                SetState<EnemyAttackStateComponent>();
            }

            if (EnemyComponent.IsPlayerFollowable)
            {
                SetState<EnemyFollowStateComponent>();
            }

            if (IsAtGuardPosition)
            {
                EnemyComponent.SetVelocity(Vector2.zero);
            }
            else
            {
                EnemyComponent.SetVelocity(PathfindingComponent.CurrentDirection * moveSpeed);
                if (Rigidbody.velocity != Vector2.zero) transform.rotation = EnemyComponent.WalkRotation;
            }
        }

        public override void OnHealthChanged(object sender, HealthEventArgs e)
        {
            base.OnHealthChanged(sender, e);

            transform.rotation = EnemyComponent.FollowRotation;
        }
    }
}
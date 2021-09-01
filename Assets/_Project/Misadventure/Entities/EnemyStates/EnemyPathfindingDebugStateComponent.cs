using Misadventure.Extensions;
using Misadventure.Level;
using Misadventure.Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Misadventure.Entities.EnemyStates
{
    public class EnemyPathfindingDebugStateComponent : EnemyBaseStateComponent
    {
        /// <summary>
        /// The movement speed of the enemy while searching.
        /// </summary>
        [SerializeField] float moveSpeed;
        [SerializeField] InputAction pathfindingAction;
        
        Vector3Int TargetPosition { get; set; }

        protected override void Awake()
        {
            base.Awake();

            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            
            pathfindingAction.performed += OnButtonPathfinding;
            pathfindingAction.Enable();
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            GameComponent.LevelLoaded -= OnLevelLoaded;
            GameComponent.LevelUnloaded -= OnLevelUnloaded;
            
            pathfindingAction.performed -= OnButtonPathfinding;
            pathfindingAction.Disable();
        }

        public override void UpdateState()
        {
            base.UpdateState();

#if UNITY_EDITOR
            Debug.DrawLine(transform.position, TargetPosition, Color.yellow);
#endif
        }

        public override void FixedUpdateState()
        {
            EnemyComponent.SetVelocity(PathfindingComponent.CurrentDirection * moveSpeed);
            if (Rigidbody.velocity != Vector2.zero) transform.rotation = EnemyComponent.WalkRotation;
        }

        void OnButtonPathfinding(InputAction.CallbackContext context)
        {
            PathfindingComponent.ClearDestination();
            
            var targetPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            TargetPosition = Locator.LevelComponent.Grid.WorldToCell(targetPosition);
            PathfindingComponent.SetDestination(TargetPosition);
        }

        void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            var navMap = Pathfinder.NavMap;
            foreach (var cellPosition in navMap.Keys)
            {
                Gizmos.DrawCube(cellPosition.ToWorld(), Vector3.one);   
            }
#endif
        }
    }
}
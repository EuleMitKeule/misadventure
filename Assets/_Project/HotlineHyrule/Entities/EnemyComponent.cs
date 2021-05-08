using System;
using System.Collections.Generic;
using HotlineHyrule.Entities.EnemyStates;
using HotlineHyrule.Extensions;
using UnityEngine;

namespace HotlineHyrule.Entities
{
    /// <summary>
    /// prototype; only contains a respawn point for now
    /// </summary>
    public class EnemyComponent : MonoBehaviour
    {
        /// <summary>
        /// The enemy's respawn point.
        /// </summary>
        [SerializeField] public Vector2Int respawnPoint;
        /// <summary>
        /// The enemy's current state
        /// </summary>
        [SerializeField] public EnemyStateBaseComponent state;
        [SerializeField] float wallCheckDistance;
        [SerializeField] LayerMask wallMask;
        [SerializeField] public Collider2D sightRangeCollider;
        [SerializeField] public ItemDropDictionary itemDrops;
        [SerializeField] float attackRange;
        [SerializeField] float followRange;
        [SerializeField] float followAngle;

        public Vector3 PlayerPosition => Locator.PlayerComponent.transform.position;
        public Vector3 PlayerDirection => (PlayerPosition - transform.position).normalized;
        public bool IsPlayerInFollowRange => (transform.position - PlayerPosition).magnitude <= followRange;
        public bool IsPlayerInAttackRange => (transform.position - PlayerPosition).magnitude <= attackRange;
        public float PlayerAngle => Vector3.SignedAngle(transform.up, PlayerDirection, Vector3.forward);
        public bool IsPlayerInAngle => Mathf.Abs(PlayerAngle) < followAngle;
        public bool IsPlayerBehindWall =>
            Physics2D.Raycast(
                transform.position,
                PlayerDirection,
                followRange,
                1 << PhysicsLayer.WALL
            );
        public bool IsPlayerVisible => IsPlayerInFollowRange && IsPlayerInAngle && !IsPlayerBehindWall;
        public bool IsPlayerAttackable => IsPlayerInAttackRange && IsPlayerInAngle && !IsPlayerBehindWall;

        public bool HasWallLeft =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                -transform.right,
                wallCheckDistance,
                wallMask
            );
        public bool HasWallRight =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                transform.right,
                wallCheckDistance,
                wallMask
            );
        public bool HasWallAbove =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                transform.up,
                wallCheckDistance,
                wallMask
            );

        Collider2D Collider { get; set; }
        HealthComponent HealthComponent { get; set; }
        public EnemyStateBaseComponent PatrolState { get; private set; }
        public EnemyStateBaseComponent SearchState { get; private set; }
        public EnemyStateBaseComponent TurnAroundState { get; private set; }
        public EnemyStateBaseComponent AttackState { get; private set; }
        public EnemyStateBaseComponent FollowState { get; private set; }
        public EnemyStateBaseComponent DyingState { get; private set; }

        void Awake()
        {
            Collider = GetComponent<Collider2D>();
            HealthComponent = GetComponent<HealthComponent>();
            PatrolState = GetComponent<EnemyStatePatrolComponent>();
            SearchState = GetComponent<EnemyStateSearchComponent>();
            TurnAroundState = GetComponent<EnemyStateTurnAroundComponent>();
            AttackState = GetComponent<EnemyStateAttackComponent>();
            FollowState = GetComponent<EnemyStateFollowComponent>();
            DyingState = GetComponent<EnemyStateDyingComponent>();

            HealthComponent.HealthChanged += OnHealthChanged;
        }

        void Start()
        {
            ChangeState(PatrolState);
        }

        void FixedUpdate()
        {
            if (state) state.FixedStateUpdate();
        }

        void Update()
        {
            if (state) state.StateUpdate();
        }

        /// <summary>
        /// Changes the enemy's state. Also exits the current one and sets up the new one
        /// </summary>
        /// <param name="newState">The new state the enemy shall get</param>
        public void ChangeState(EnemyStateBaseComponent newState)
        {
            Debug.Log(newState.GetType().Name);

            if (!newState) return;
            if (state && newState.priority < state.priority) return;
            if (state) state.Exit();
            state = newState;
            state.Setup();
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (e.NewHealth > 0) return;
            ChangeState(DyingState);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            // Check if target is Player and if there is no wall in the way
            var dir = other.transform.position - transform.position;
            if (other.gameObject.layer != LayerMask.NameToLayer("player")
             || Physics2D.Raycast(transform.position, dir, dir.magnitude, wallMask)) return;
            
            // Look at player
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            state.OnLookingAtPlayer();
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("enemy")) return;
            transform.Rotate(Vector3.forward, 90f);
        }
    }
}
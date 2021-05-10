using System;
using System.Collections.Generic;
using HotlineHyrule.Entities.EnemyStates;
using HotlineHyrule.Extensions;
using HotlineHyrule.Items;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [Header("General")]
        [SerializeField] public Vector2Int respawnPoint;
        /// <summary>
        /// List of items that can be dropped by a given chance when the enemy is destroyed.
        /// </summary>
        [SerializeField] List<ItemDrop> itemDrops;

        /// <summary>
        /// The enemy's current state.
        /// </summary>
        [Header("AI")]
        [SerializeField] public EnemyStateBaseComponent state;
        /// <summary>
        /// Layers that count as wall.
        /// </summary>
        [SerializeField] LayerMask wallMask;
        /// <summary>
        /// Distance to check for walls at.
        /// </summary>
        [SerializeField] float wallCheckDistance;
        /// <summary>
        /// Total angle of the vision cone.
        /// </summary>
        [SerializeField] float followAngle;
        /// <summary>
        /// Range in which the enemy will follow.
        /// </summary>
        [SerializeField] float followRange;
        /// <summary>
        /// Range in which the enemy will attack.
        /// </summary>
        [SerializeField] float attackRange;

        /// <summary>
        /// Particle system prefab to spawn when taking damage.
        /// </summary>
        [Header("Effects")]
        [SerializeField] GameObject damageParticleSystemPrefab;

        public Vector3 PlayerPosition => Locator.PlayerComponent.transform.position;
        public Vector3 PlayerDirection => transform.position.DirectionTo(PlayerPosition);

        float WalkAngle => Vector3.SignedAngle(Vector3.up, Rigidbody.velocity, Vector3.forward);
        float FollowAngle => Vector3.SignedAngle(Vector3.up, PlayerDirection, Vector3.forward);
        float PlayerAngle => Vector3.SignedAngle(transform.up, PlayerDirection, Vector3.forward);
        public Quaternion WalkRotation => Quaternion.Euler(0f, 0f, WalkAngle);
        public Quaternion FollowRotation => Quaternion.Euler(0f, 0f, FollowAngle);

        bool IsPlayerInAngle => Mathf.Abs(PlayerAngle) < followAngle;
        bool IsPlayerInFollowRange => transform.position.DistanceTo(PlayerPosition) <= followRange;
        bool IsPlayerInAttackRange => transform.position.DistanceTo(PlayerPosition) <= attackRange;
        public bool IsPlayerFollowable => IsPlayerInFollowRange && IsPlayerInAngle && IsPlayerVisible;
        public bool IsPlayerAttackable => IsPlayerInAttackRange && IsPlayerInAngle && IsPlayerVisible;

        bool IsPlayerVisible =>
            Physics2D.Raycast(
                transform.position,
                PlayerDirection,
                followRange,
                wallMask | 1 << PhysicsLayer.PLAYER
            ).transform.gameObject.layer.IsPlayer();

        public bool IsWallLeft =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                -transform.right,
                wallCheckDistance,
                wallMask
            );
        public bool IsWallRight =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                transform.right,
                wallCheckDistance,
                wallMask
            );
        public bool IsWallAbove =>
            Physics2D.BoxCast(
                transform.position,
                Collider.bounds.size,
                0f,
                transform.up,
                wallCheckDistance,
                wallMask
            );

        Rigidbody2D Rigidbody { get; set; }
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
            Rigidbody = GetComponent<Rigidbody2D>();
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
            if (state) state.StateFixedUpdate();
        }

        void Update()
        {
            if (state) state.StateUpdate();

#if UNITY_EDITOR
            if (IsPlayerFollowable) Debug.DrawLine(transform.position, PlayerPosition, IsPlayerAttackable ? Color.green : Color.red);
#endif
        }

        /// <summary>
        /// Changes the enemy's state. Also exits the current one and sets up the new one
        /// </summary>
        /// <param name="newState">The new state the enemy shall get</param>
        public void ChangeState(EnemyStateBaseComponent newState)
        {
            if (!newState) return;
            if (state && newState.priority < state.priority) return;
            if (state) state.ExitState();

            state = newState;
            state.EnterState();
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (e.IsDamage)
            {
                if (damageParticleSystemPrefab) Instantiate(damageParticleSystemPrefab, transform.position, Quaternion.identity);
            }

            if (e.IsKilled)
            {
                ChangeState(DyingState);

                foreach (var item in itemDrops)
                {
                    if (Random.value <= item.dropRate)
                    {
                        Instantiate(item.data.itemPrefab, transform.position, Quaternion.identity);
                    }
                }
            }
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (state) state.StateOnCollisionEnter2D(other);
        }
        
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (state != null)
            {
                var style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.white;
                
                Handles.Label(transform.position, state.GetType().Name, style);
            }
        }
#endif
    }
}
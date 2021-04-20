using System;
using System.Collections.Generic;
using HotlineHyrule.Entities.EnemyStates;
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

        public bool HasWallLeft =>
            Physics2D.BoxCast(
                transform.position,
                _collider.bounds.size,
                0f,
                -transform.right,
                wallCheckDistance,
                wallMask
            );
        public bool HasWallRight =>
            Physics2D.BoxCast(
                transform.position,
                _collider.bounds.size,
                0f,
                transform.right,
                wallCheckDistance,
                wallMask
            );
        public bool HasWallAbove =>
            Physics2D.BoxCast(
                transform.position,
                _collider.bounds.size,
                0f,
                transform.up,
                wallCheckDistance,
                wallMask
            );

        public EnemyStateBaseComponent PatrolState { get; private set; }

        Collider2D _collider;

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            PatrolState = GetComponent<EnemyStatePatrolComponent>();
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
            if (!newState) return;

            if (state) state.Exit();
            state = newState;
            state.Setup();
        }
    }
}
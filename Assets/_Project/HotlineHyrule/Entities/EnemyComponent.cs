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

        [HideInInspector] public EnemyStateBaseComponent patrolState;

        void Awake()
        {
            patrolState = GetComponent<EnemyStatePatrolComponent>();
        }

        void Start()
        {
            ChangeState(patrolState);
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
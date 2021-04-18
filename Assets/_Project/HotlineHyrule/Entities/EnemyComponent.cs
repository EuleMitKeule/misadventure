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
        /// The enemy's possible states
        /// </summary>
        [System.Serializable]
        public enum EnemyState
        {
            Idle,
            Watch,
            Patrol,
            Defend,
            Attack,
            Dying,
            Dead
        }

        Dictionary<EnemyState, EnemyStateBaseComponent> stateComponentsMap;

        /// <summary>
        /// The enemy's current state
        /// </summary>
        public EnemyState state = EnemyState.Patrol;

        void Awake()
        {
            stateComponentsMap = new Dictionary<EnemyState, EnemyStateBaseComponent>()
            {
                {EnemyState.Patrol, GetComponent<EnemyStatePatrolComponent>()}
            };
        }

        void FixedUpdate()
        {
            stateComponentsMap[state].FixedStateUpdate();
        }

        void Update()
        {
            stateComponentsMap[state].StateUpdate();
        }

        /// <summary>
        /// Changes the enemy's state. Also exits the current one and sets up the new one
        /// </summary>
        /// <param name="newState">The new state the enemy shall get</param>
        void ChangeState(EnemyState newState)
        {
            stateComponentsMap[state].Exit();
            state = newState;
            stateComponentsMap[state].Setup();
        }
    }
}
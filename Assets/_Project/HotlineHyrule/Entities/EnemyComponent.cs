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
        [SerializeField] float playerCheckDistance;
        [SerializeField] LayerMask wallMask;
        [SerializeField] LayerMask playerMask;
        [SerializeField] public Collider2D sightRangeCollider;

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

        void Awake()
        {
            Collider = GetComponent<Collider2D>();
            HealthComponent = GetComponent<HealthComponent>();
            PatrolState = GetComponent<EnemyStatePatrolComponent>();

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
            if (!newState) return;

            if (state) state.Exit();
            state = newState;
            state.Setup();
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (e.NewHealth > 0) return;
            
            Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            Vector3 dir = other.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (state) state.OnLookingAtPlayer();
            // int mask = playerMask.value;
            // mask = ~mask;
            // Debug.Log(mask);
            //
            // RaycastHit hit;
            // // Does the ray intersect any objects excluding the player layer
            // if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit))
            // {
            //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow, 10f);
            //     Debug.Log("HIT " + hit.distance);
            // }
            // else
            // {
            //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1000, Color.white, 10f);
            //     Debug.Log("NOT HIT " + hit.distance);
            // }
        }
    }
}
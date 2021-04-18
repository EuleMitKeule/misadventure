using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HotlineHyrule.Entities.EnemyStates
{
    [RequireComponent(typeof(EnemyComponent))]
    public class EnemyStatePatrolComponent : EnemyStateBaseComponent
    {
        [SerializeField] float moveSpeed = 100f;
        [SerializeField] float collisionCooldown = 1f;
        
        EnemyComponent _enemyComponent;
        Rigidbody2D _rb;
        float _collisionTimer;

        void Awake()
        {
            _enemyComponent = GetComponent<EnemyComponent>();
            _rb = GetComponent<Rigidbody2D>();
        }

        public override void Setup()
        {
            
        }

        public override void StateUpdate()
        {
        }

        public override void FixedStateUpdate()
        {
            if (_enemyComponent.state != EnemyComponent.EnemyState.Patrol) return;
            _rb.velocity = transform.up * moveSpeed * Time.deltaTime;
        }

        public override void Exit()
        {
            
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (LayerMask.LayerToName(other.gameObject.layer) != "wall") return;
            _collisionTimer = Math.Max(0f, _collisionTimer - Time.deltaTime);
            if (_collisionTimer > 0f) return;
            var angle = 90f;
            if (Random.value >= 0.5f) angle *= -1f;
            transform.eulerAngles += Vector3.forward * angle;
            _collisionTimer = collisionCooldown;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            _collisionTimer = 0f;
        }
    }
}
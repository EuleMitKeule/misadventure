using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HotlineHyrule.Entities.EnemyStates
{
    [RequireComponent(typeof(EnemyComponent))]
    public class EnemyStatePatrolComponent : EnemyStateBaseComponent
    {
        [SerializeField] float moveSpeed;

        Rigidbody2D _rb;
        EnemyComponent _enemyComponent;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _enemyComponent = GetComponent<EnemyComponent>();
        }

        public override void FixedStateUpdate()
        {
            _rb.velocity = transform.up * moveSpeed;

            if (!_enemyComponent.HasWallAbove) return;

            if (_enemyComponent.HasWallLeft &! _enemyComponent.HasWallRight)
            {
                transform.eulerAngles += Vector3.forward * -90f;
                return;
            }

            if (_enemyComponent.HasWallRight &! _enemyComponent.HasWallLeft)
            {
                transform.eulerAngles += Vector3.forward * 90f;
                return;
            }

            if (_enemyComponent.HasWallLeft && _enemyComponent.HasWallRight)
            {
                transform.eulerAngles += Vector3.forward * 180f;
            }

            var isTurningLeft = Random.Range(0, 2) == 1;
            transform.eulerAngles += Vector3.forward * (isTurningLeft ? 90f : -90f);
        }
    }
}
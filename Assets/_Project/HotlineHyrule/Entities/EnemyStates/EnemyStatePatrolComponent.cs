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

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            enemyComponent = GetComponent<EnemyComponent>();
        }

        public override void FixedStateUpdate()
        {
            _rb.velocity = transform.up * moveSpeed;

            if (!enemyComponent.HasWallAbove) return;

            if (enemyComponent.HasWallLeft &! enemyComponent.HasWallRight)
            {
                transform.eulerAngles += Vector3.forward * -90f;
                return;
            }

            if (enemyComponent.HasWallRight &! enemyComponent.HasWallLeft)
            {
                transform.eulerAngles += Vector3.forward * 90f;
                return;
            }

            if (enemyComponent.HasWallLeft && enemyComponent.HasWallRight)
            {
                transform.eulerAngles += Vector3.forward * 180f;
            }

            var isTurningLeft = Random.Range(0, 2) == 1;
            transform.eulerAngles += Vector3.forward * (isTurningLeft ? 90f : -90f);
        }
    }
}
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HotlineHyrule.Entities.EnemyStates
{
    [RequireComponent(typeof(EnemyComponent))]
    public class EnemyStatePatrolComponent : EnemyStateBaseComponent
    {
        /// <summary>
        /// Move speed of the enemy
        /// </summary>
        [SerializeField] float moveSpeed;
        /// <summary>
        /// Cooldown for sight range collider. It makes sense to set one to avoid continuous state changes
        /// </summary>
        [SerializeField] float sightRangeColliderCooldown = 2f;

        Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            //enemyComponent = GetComponent<EnemyComponent>();
        }

        public override void Setup()
        {
            base.Setup();
            StartCoroutine(DisableSightRangeCollider());
        }

        IEnumerator DisableSightRangeCollider()
        {
            enemyComponent.sightRangeCollider.enabled = false;
            yield return new WaitForSeconds(sightRangeColliderCooldown);
            enemyComponent.sightRangeCollider.enabled = true;
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

        public override void Exit()
        {
            base.Exit();
            _rb.velocity = Vector2.zero;
        }

        public override void OnLookingAtPlayer()
        {
            base.OnLookingAtPlayer();
            enemyComponent.ChangeState(enemyComponent.ShootProjectileState);
        }
    }
}
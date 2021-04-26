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

        public override void Setup()
        {
            base.Setup();

            if (Animator) Animator.SetBool("isMoving", true);
        }

        public override void FixedStateUpdate()
        {
            Rigidbody.velocity = transform.up * moveSpeed;

            if (!EnemyComponent.HasWallAbove) return;

            if (EnemyComponent.HasWallLeft &! EnemyComponent.HasWallRight)
            {
                transform.eulerAngles += Vector3.forward * -90f;
                return;
            }

            if (EnemyComponent.HasWallRight &! EnemyComponent.HasWallLeft)
            {
                transform.eulerAngles += Vector3.forward * 90f;
                return;
            }

            if (EnemyComponent.HasWallLeft && EnemyComponent.HasWallRight)
            {
                transform.eulerAngles += Vector3.forward * 180f;
            }

            var isTurningLeft = Random.Range(0, 2) == 1;
            transform.eulerAngles += Vector3.forward * (isTurningLeft ? 90f : -90f);
        }

        public override void Exit()
        {
            base.Exit();
            Rigidbody.velocity = Vector2.zero;
        }

        public override void OnLookingAtPlayer()
        {
            base.OnLookingAtPlayer();
            EnemyComponent.ChangeState(EnemyComponent.ShootProjectileState);
        }
    }
}
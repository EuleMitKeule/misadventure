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

        public override void EnterState()
        {
            base.EnterState();

            if (Animator) Animator.SetBool("isMoving", true);
        }

        public override void ExitState()
        {
            base.ExitState();
            
            Rigidbody.velocity = Vector2.zero;
        }

        public override void OnCollisionEnterState(Collision2D other)
        {
            if (!other.gameObject.layer.IsEnemy()) return;

            transform.Rotate(Vector3.forward, 90f);
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            
            Rigidbody.velocity = transform.up * moveSpeed;

            if (EnemyComponent.IsPlayerFollowable)
            {
                if (EnemyComponent.FollowState)
                {
                    EnemyComponent.ChangeState(EnemyComponent.FollowState);
                }
            }

            if (EnemyComponent.IsWallAbove)
            {
                if (EnemyComponent.IsWallLeft &! EnemyComponent.IsWallRight)
                {
                    transform.eulerAngles += Vector3.forward * -90f;
                    return;
                }

                if (EnemyComponent.IsWallRight &! EnemyComponent.IsWallLeft)
                {
                    transform.eulerAngles += Vector3.forward * 90f;
                    return;
                }

                if (EnemyComponent.IsWallLeft && EnemyComponent.IsWallRight)
                {
                    transform.eulerAngles += Vector3.forward * 180f;
                }

                var isTurningLeft = Random.Range(0, 2) == 1;
                transform.eulerAngles += Vector3.forward * (isTurningLeft ? 90f : -90f);
            }
        }
    }
}
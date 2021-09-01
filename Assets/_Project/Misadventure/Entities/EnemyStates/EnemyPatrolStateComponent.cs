using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HotlineHyrule.Entities.EnemyStates
{
    [RequireComponent(typeof(EnemyComponent))]
    public class EnemyPatrolStateComponent : EnemyBaseStateComponent
    {
        /// <summary>
        /// The movement speed of the enemy while patrolling.
        /// </summary>
        [SerializeField] float moveSpeed;

        public override void ExitState()
        {
            base.ExitState();

            EnemyComponent.SetVelocity(Vector2.zero);
        }

        public override void OnCollisionEnterState(Collision2D other)
        {
            if (!other.gameObject.layer.IsEnemy()) return;

            transform.Rotate(Vector3.forward, 90f);
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();

            EnemyComponent.SetVelocity(transform.up * moveSpeed);

            if (EnemyComponent.IsPlayerFollowable)
            {
                SetState<EnemyFollowStateComponent>();
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

        public override void OnHealthChanged(object sender, HealthEventArgs e)
        {
            transform.rotation = EnemyComponent.FollowRotation;
        }
    }
}
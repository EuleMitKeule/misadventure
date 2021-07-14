using System;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyIdleStateComponent : EnemyBaseStateComponent
    {
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

            if (EnemyComponent.IsPlayerFollowable)
            {
                SetState<EnemyFollowStateComponent>();
            }
        }

        public override void OnHealthChanged(object sender, HealthEventArgs e)
        {
            transform.rotation = EnemyComponent.FollowRotation;
        }
    }
}
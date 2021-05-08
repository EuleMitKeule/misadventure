using HotlineHyrule.Entities.EnemyStates.HelperComponents;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyStateFollowComponent : EnemyStateBaseComponent
    {
        // /// <summary>
        // /// Duration for the enemy following the player
        // /// </summary>
        // [SerializeField] float followDuration;
        /// <summary>
        /// Move speed for enemy while following the player
        /// </summary>
        [SerializeField] float moveSpeed;
        // [SerializeField] float attackRange;
        //
        // float _timer;

        // Vector3 LookDirection => transform.position - PlayerObject.transform.position;
        // float LookAngle => Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg + 90f;
        // Quaternion LookRotation => Quaternion.Euler(0f, 0f, LookAngle);

        public override void FixedStateUpdate()
        {
            base.FixedStateUpdate();

            // transform.LookAt(EnemyComponent.PlayerPosition);
            Rigidbody.velocity = EnemyComponent.PlayerDirection * moveSpeed;

            if (EnemyComponent.IsPlayerAttackable)
            {
                EnemyComponent.ChangeState(EnemyComponent.AttackState);
            }

            if (!EnemyComponent.IsPlayerVisible)
            {
                EnemyComponent.ChangeState(EnemyComponent.SearchState);
            }
        }
    }
}
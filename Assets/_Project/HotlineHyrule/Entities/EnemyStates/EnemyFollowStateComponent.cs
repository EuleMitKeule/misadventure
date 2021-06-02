using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyFollowStateComponent : EnemyBaseStateComponent
    {
        /// <summary>
        /// The movement speed for enemy while following.
        /// </summary>
        [SerializeField] float moveSpeed;
        
        public override void FixedUpdateState()
        {
            base.FixedUpdateState();

            EnemyComponent.SetVelocity(EnemyComponent.PlayerDirection * moveSpeed);
            transform.rotation = EnemyComponent.FollowRotation;

            if (EnemyComponent.IsPlayerAttackable)
            {
                EnemyComponent.ChangeState(EnemyComponent.AttackState);
            }

            if (!EnemyComponent.IsPlayerFollowable)
            {
                EnemyComponent.ChangeState(EnemyComponent.SearchState);
            }
        }
    }
}
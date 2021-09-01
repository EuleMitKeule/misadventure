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

            HandleStateRouting();
        }

        protected virtual void HandleStateRouting()
        {
            if (EnemyComponent.IsPlayerAttackable)
            {
                SetState<EnemyAttackStateComponent>();
            }

            if (!EnemyComponent.IsPlayerFollowable)
            {
                var nextState =
                    States.Find(e => e is EnemySearchStateComponent)?.GetType() ?? 
                    EnemyComponent.PassiveState;
                
                SetState(nextState);
            }
        }
    }
}
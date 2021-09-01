using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{

    public class EnemyDyingStateComponent : EnemyBaseStateComponent
    {
        public override void EnterState()
        {
            base.EnterState();

            EnemyComponent.SetVelocity(Vector2.zero);
            Rigidbody.simulated = false;

            if (Animator) Animator.SetTrigger("dying");
        }
    }
}
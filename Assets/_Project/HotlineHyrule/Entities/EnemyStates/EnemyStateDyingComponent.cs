using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{

    public class EnemyStateDyingComponent : EnemyStateBaseComponent
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
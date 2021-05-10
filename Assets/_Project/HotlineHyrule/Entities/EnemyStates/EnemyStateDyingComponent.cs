using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{

    public class EnemyStateDyingComponent : EnemyStateBaseComponent
    {
        public override void EnterState()
        {
            base.EnterState();

            Rigidbody.simulated = false;

            if (Animator)
            {
                Animator.SetBool("isMoving", true);
                Animator.SetTrigger("dying");
            }
        }
    }
}
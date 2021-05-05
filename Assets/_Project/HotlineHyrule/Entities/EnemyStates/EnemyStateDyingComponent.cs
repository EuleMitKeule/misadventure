using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{

    public class EnemyStateDyingComponent : EnemyStateBaseComponent
    {
        public override void Setup()
        {
            base.Setup();
            Animator.SetTrigger("dying");
        }
    }
}
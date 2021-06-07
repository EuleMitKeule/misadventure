using System.Collections;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyAttackMeleeStateComponent : EnemyAttackStateComponent
    {
        protected override IEnumerator AttackRoutine()
        {
            if (Animator) Animator.SetTrigger(EnemyComponent.UsingAttack2 ? "attack2" : "attack");
            if (WeaponComponent) WeaponComponent.PerformAttack();
            EnemyComponent.ChangeState(EnemyComponent.FollowState);
            yield return null;
        }
    }
}
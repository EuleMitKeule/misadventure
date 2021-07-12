using System.Collections;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyAttackMeleeStateComponent : EnemyAttackStateComponent
    {
        protected override IEnumerator AttackRoutine()
        {
            if (Animator) Animator.SetTrigger(EnemyComponent.AttackAnimationTrigger);
            if (WeaponComponent) WeaponComponent.PerformAttack();
            SetState<EnemyFollowStateComponent>();
            yield return null;
        }
    }
}
using System.Collections;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class EnemyAttackMeleeStateComponent : EnemyAttackStateComponent
    {
        protected override IEnumerator AttackRoutine()
        {
            if (Animator) Animator.SetTrigger(EnemyComponent.attackAnimationTrigger);
            if (WeaponComponent) WeaponComponent.PerformAttack();
            SetState<EnemyFollowStateComponent>();
            yield return null;
        }
    }
}
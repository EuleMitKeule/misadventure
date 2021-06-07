using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    public class NinjaFollowStateComponent : EnemyFollowStateComponent
    {
        [SerializeField] WeaponData attack1Weapon;
        [SerializeField] WeaponData attack2Weapon;
        /// <summary>
        /// The maximum distance towards the target to trigger Attack2.
        /// If the distance is higher, Attack1 will be triggered instead
        /// </summary>
        [SerializeField] float maxDistanceForUsingAttack2;
        [SerializeField] float maxDistanceForPerformingAttack2;
        //[SerializeField] EnemyAttackMeleeStateComponent enemyAttackMeleeStateComponent;
        
        protected override void HandleStateRouting()
        {
            if (EnemyComponent.IsPlayerAttackable)
            {
                if (EnemyComponent.PlayerDistance > maxDistanceForUsingAttack2)
                {
                    EnemyComponent.UsingAttack2 = false;
                    EnemyComponent.WeaponComponent.SetWeapon(attack1Weapon);
                    EnemyComponent.ChangeState(EnemyComponent.AttackState);
                }
                else if (EnemyComponent.PlayerDistance <= maxDistanceForPerformingAttack2)
                {
                    EnemyComponent.UsingAttack2 = true;
                    EnemyComponent.WeaponComponent.SetWeapon(attack2Weapon);
                    EnemyComponent.ChangeState(EnemyComponent.AttackState);
                }
            }

            if (!EnemyComponent.IsPlayerFollowable)
            {
                EnemyComponent.ChangeState(EnemyComponent.SearchState);
            }
        }
    }
}
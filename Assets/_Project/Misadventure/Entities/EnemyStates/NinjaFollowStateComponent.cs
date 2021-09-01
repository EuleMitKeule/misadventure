using Misadventure.Weapons;
using UnityEngine;

namespace Misadventure.Entities.EnemyStates
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
        
        protected override void HandleStateRouting()
        {
            if (EnemyComponent.IsPlayerAttackable)
            {
                if (EnemyComponent.PlayerDistance > maxDistanceForUsingAttack2)
                {
                    EnemyComponent.AttackAnimationTrigger = "attack";
                    EnemyComponent.WeaponComponent.SetWeapon(attack1Weapon);
                    SetState<EnemyAttackStateComponent>();
                }
                else if (EnemyComponent.PlayerDistance <= maxDistanceForPerformingAttack2)
                {
                    EnemyComponent.AttackAnimationTrigger = "attack2";
                    EnemyComponent.WeaponComponent.SetWeapon(attack2Weapon);
                    SetState<EnemyAttackStateComponent>();
                }
            }

            if (!EnemyComponent.IsPlayerFollowable)
            {
                SetState<EnemySearchStateComponent>();
            }
        }
    }
}
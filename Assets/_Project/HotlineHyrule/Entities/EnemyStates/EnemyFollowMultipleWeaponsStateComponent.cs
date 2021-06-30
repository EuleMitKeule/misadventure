using System.Collections.Generic;
using HotlineHyrule.Weapons;
using UnityEngine;

namespace HotlineHyrule.Entities.EnemyStates
{
    [System.Serializable]
    public struct WeaponAnimation
    {
        public WeaponData data;
        public string animationTrigger;
    }
    
    public class EnemyFollowMultipleWeaponsStateComponent : EnemyFollowStateComponent
    {
        /// <summary>
        /// Weapon data + animation triggers that shall be used for the corresponding attack
        /// </summary>
        [SerializeField] List<WeaponAnimation> weapons;

        protected override void HandleStateRouting()
        {
            if (EnemyComponent.IsPlayerAttackable)
            {
                var attackIndex = Random.Range(0, weapons.Count);
                EnemyComponent.attackAnimationTrigger = weapons[attackIndex].animationTrigger;
                EnemyComponent.WeaponComponent.SetWeapon(weapons[attackIndex].data);
                SetState<EnemyAttackStateComponent>();
            }

            if (!EnemyComponent.IsPlayerFollowable)
            {
                SetState<EnemySearchStateComponent>();
            }
        }
    }
}
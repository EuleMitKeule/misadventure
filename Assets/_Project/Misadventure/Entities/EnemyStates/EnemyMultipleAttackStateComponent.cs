using System.Collections;
using System.Collections.Generic;
using Misadventure.Weapons;
using UnityEngine;

namespace Misadventure.Entities.EnemyStates
{
    [System.Serializable]
    public struct WeaponAnimation
    {
        public WeaponData data;
        public string animationTrigger;
    }
    
    public class EnemyMultipleAttackStateComponent : EnemyAttackStateComponent
    {
        /// <summary>
        /// Weapon data + animation triggers that shall be used for the corresponding attack
        /// </summary>
        [SerializeField] List<WeaponAnimation> weapons;
        
        protected override IEnumerator AttackRoutine()
        {
            while (true)
            {
                var attackIndex = Random.Range(0, weapons.Count);
                EnemyComponent.AttackAnimationTrigger = weapons[attackIndex].animationTrigger;
                EnemyComponent.WeaponComponent.SetWeapon(weapons[attackIndex].data);
                
                if (WeaponComponent.CanAttack)
                {
                    if (Animator) Animator.SetTrigger(EnemyComponent.AttackAnimationTrigger);
                    if (WeaponComponent) WeaponComponent.PerformAttack();   
                }

                yield return new WaitForSeconds(WeaponComponent.AttackDelay);
            }
        }
    }
}
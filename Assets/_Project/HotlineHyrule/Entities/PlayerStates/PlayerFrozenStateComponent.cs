using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Entities.PlayerStates
{
    public class PlayerFrozenStateComponent : PlayerBaseStateComponent
    {

        public override void EnterState()
        {
            base.EnterState();

            HealthComponent.IsInvincible = true;

            PlayerComponent.walkAction.Disable();
            PlayerComponent.dodgeAction.Disable();
            WeaponComponent.attackAction.Disable();
            LoadoutComponent.changeWeaponAction.Disable();
            ItemPickupComponent.pickupAction.Disable();
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();

            Rigidbody.velocity = Vector2.zero;
        }

        public override void ExitState()
        {
            base.ExitState();

            HealthComponent.IsInvincible = false;

            PlayerComponent.walkAction.Enable();
            PlayerComponent.dodgeAction.Enable();
            WeaponComponent.attackAction.Enable();
            LoadoutComponent.changeWeaponAction.Enable();
            ItemPickupComponent.pickupAction.Enable();
        }
    }
}
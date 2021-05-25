using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Entities.PlayerStates
{
    public class PlayerDeathStateComponent : PlayerBaseStateComponent
    {

        public override void EnterState()
        {
            base.EnterState();

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
    }
}
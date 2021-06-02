using UnityEngine;

namespace HotlineHyrule.Entities.PlayerStates
{
    public class PlayerDodgeStateComponent : PlayerBaseStateComponent
    {

        public override void EnterState()
        {
            var dodgeDirection = PlayerComponent.IsMoving ? PlayerComponent.WalkAxis : (Vector2)transform.up;
            Rigidbody.AddForce(dodgeDirection * PlayerComponent.dodgeForce, ForceMode2D.Impulse);

            Invoke(nameof(ChangeState), PlayerComponent.dodgeDuration);
        }

        public override void FixedUpdateState()
        {

        }

        void ChangeState() => SetState<PlayerIdleStateComponent>();
    }
}
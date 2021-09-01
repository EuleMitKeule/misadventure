namespace Misadventure.Entities.PlayerStates
{
    public class PlayerIdleStateComponent : PlayerBaseStateComponent
    {

        public override void EnterState()
        {

        }

        public override void FixedUpdateState()
        {
            Rigidbody.velocity = PlayerComponent.WalkAxis * (PlayerComponent.movementSpeed *
                                                             PlayerComponent.MovementAttackFactor *
                                                             PlayerComponent.MovementItemFactor);
        }
    }
}
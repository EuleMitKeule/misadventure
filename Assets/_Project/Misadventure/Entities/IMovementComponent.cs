using Misadventure.Items;

namespace Misadventure.Entities
{
    public interface IMovementComponent
    {
        void Consume(MovementItemData movementItem);
    }
}
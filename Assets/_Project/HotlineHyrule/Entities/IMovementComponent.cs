using System;
using HotlineHyrule.Items;

namespace HotlineHyrule.Entities
{
    public interface IMovementComponent
    {
        void Consume(MovementItemData movementItem);
    }
}
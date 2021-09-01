using UnityEngine;

namespace Misadventure.Items
{
    [CreateAssetMenu(menuName = "Item/New Movement Item")]
    public class MovementItemData : ConsumableItemData
    {
        /// <summary>
        /// Multiplies the absolute movement speed of the entity.
        /// </summary>
        [SerializeField] public float movementFactor;
        /// <summary>
        /// The amount of time the item's effect should last.
        /// </summary>
        [SerializeField] public int duration;
    }
}
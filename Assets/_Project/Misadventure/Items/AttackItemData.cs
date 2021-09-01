using UnityEngine;

namespace HotlineHyrule.Items
{
    [CreateAssetMenu(menuName = "Item/New Attack Item")]
    public class AttackItemData : ConsumableItemData
    {
        /// <summary>
        /// Multiplies the absolute damage the weapon deals.
        /// </summary>
        [SerializeField] public float damageFactor { get; set; }
        /// <summary>
        /// Adds to the absolute damage the weapon deals.
        /// </summary>
        [SerializeField] public int damageBonus;
        /// <summary>
        /// Divides the delay between attacks.
        /// </summary>
        [SerializeField] public float attackSpeed;
        /// <summary>
        /// The time the item's effect should last.
        /// </summary>
        [SerializeField] public int duration;
    }
}
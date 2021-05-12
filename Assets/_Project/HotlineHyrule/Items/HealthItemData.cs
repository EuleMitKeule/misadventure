using UnityEngine;

namespace HotlineHyrule.Items
{
    [CreateAssetMenu(menuName = "Item/New Health Item")]
    public class HealthItemData : ConsumableItemData
    {
        /// <summary>
        /// The total amount of health to gain.
        /// </summary>
        [SerializeField] public int healTotal;
        /// <summary>
        /// The amount of health to gain per interval.
        /// </summary>
        [SerializeField] public int healAmount;
        /// <summary>
        /// The interval in which to apply the heal.
        /// </summary>
        [SerializeField] public float healRate;
    }
}
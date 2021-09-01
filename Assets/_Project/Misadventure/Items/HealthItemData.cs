using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HotlineHyrule.Items
{
    [CreateAssetMenu(menuName = "Item/New Health Item")]
    public class HealthItemData : ConsumableItemData
    {
        [OdinSerialize]
        [BoxGroup("General")]
        [EnumToggleButtons]
        HealType SelectedHealType { get; set; }

        /// <summary>
        /// The total amount of health to gain.
        /// </summary>
        [OdinSerialize]
        [BoxGroup("General")]
        public int HealTotal { get; private set; }
        /// <summary>
        /// The amount of health to gain per interval.
        /// </summary>
        [BoxGroup("General")]
        [OdinSerialize]
        [ShowIf("@SelectedHealType == HealType.OverTime")]
        public int HealAmount { get; private set; }
        /// <summary>
        /// The interval in which to apply the heal.
        /// </summary>
        [BoxGroup("General")]
        [OdinSerialize]
        [ShowIf("@SelectedHealType == HealType.OverTime")]
        public float HealRate { get; set; }

        enum HealType
        {
            OneShot,
            OverTime,
        }
    }
}
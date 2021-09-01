using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Misadventure.Items
{
    public abstract class ConsumableItemData : ItemData
    {
        /// <summary>
        /// The particle system to spawn when the item is consumed.
        /// </summary>
        [BoxGroup("General")]
        [LabelText("Consume Particle Effect")]
        [OdinSerialize]
        public GameObject ConsumeParticleSystemPrefab { get; private set; }
        [BoxGroup("General")]
        [OdinSerialize]
        public string InfoText { get; private set; }

        public override string ItemName => name.Replace("item_", "");
        protected override bool IsItemNameReadOnly => true;
    }
}
using UnityEngine;

namespace HotlineHyrule.Items
{
    public abstract class ConsumableItemData : ItemData
    {
        /// <summary>
        /// The particle system to spawn when the item is consumed.
        /// </summary>
        [SerializeField] public GameObject consumeParticleSystemPrefab;

        protected override bool IsItemNameReadOnly => true;
        public override string ItemName => name.Replace("item_", "");
    }
}
using UnityEngine;

namespace HotlineHyrule.Items
{
    public class ConsumableItemData : ItemData
    {
        /// <summary>
        /// The particle system to spawn when the item is consumed.
        /// </summary>
        [SerializeField] public GameObject consumeParticleSystemPrefab;
    }
}
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Misadventure.Items
{
    [HideMonoScript]
    public class ItemData : SerializedScriptableObject
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        [OdinSerialize]
        [DisableIf("IsItemNameReadOnly")]
        [PropertyOrder(-1)]
        [BoxGroup("General")]
        public virtual string ItemName { get; set; }
        /// <summary>
        /// The prefab to spawn when dropping the item.
        /// </summary>
        [OdinSerialize]
        [PropertyOrder(-1)]
        [BoxGroup("General")]
        public GameObject ItemPrefab { get; set; }
        protected virtual bool IsItemNameReadOnly => false;
    }
}

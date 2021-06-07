using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HotlineHyrule.Items
{
    public class ItemData : SerializedScriptableObject
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        [OdinSerialize]
        [DisableIf("IsItemNameReadOnly")]
        [PropertyOrder(-1)]
        public virtual string ItemName { get; set; }
        protected virtual bool IsItemNameReadOnly => false;
        /// <summary>
        /// The prefab to spawn when dropping the item.
        /// </summary>
        [OdinSerialize]
        [PropertyOrder(-1)]
        public GameObject ItemPrefab { get; set; }
    }
}
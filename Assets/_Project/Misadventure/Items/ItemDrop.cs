using System;
using UnityEngine;

namespace Misadventure.Items
{
    /// <summary>
    /// Struct that combines given Item data with a drop rate
    /// </summary>
    [Serializable]
    public struct ItemDrop
    {
        /// <summary>
        /// The Item data that shall be dropped
        /// </summary>
        public ItemData data;

        /// <summary>
        /// The likelihood to drop the item
        /// </summary>
        [Range(0f, 1f)] public float dropRate;
    }
}
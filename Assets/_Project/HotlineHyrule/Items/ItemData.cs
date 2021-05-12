using System;
using UnityEngine;

namespace HotlineHyrule.Items
{
    public class ItemData : ScriptableObject
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        [SerializeField] public string itemName;
        /// <summary>
        /// The prefab to spawn when dropping the item.
        /// </summary>
        [SerializeField] public GameObject itemPrefab;
    }
}
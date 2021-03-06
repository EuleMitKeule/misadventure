using System.Collections.Generic;
using UnityEngine;

namespace Misadventure.Items
{
    public class ItemComponent : MonoBehaviour
    {
        /// <summary>
        /// The sub item data objects the item contains.
        /// </summary>
        [SerializeField] public List<ItemData> itemDatas = new List<ItemData>();
    }
}
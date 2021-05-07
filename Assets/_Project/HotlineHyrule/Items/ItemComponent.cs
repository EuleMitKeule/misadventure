using System.Collections.Generic;
using UnityEngine;

namespace HotlineHyrule.Items
{
    public class ItemComponent : MonoBehaviour
    {
        [SerializeField] public List<ItemData> itemDatas = new List<ItemData>();
    }
}
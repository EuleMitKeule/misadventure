using UnityEngine;

namespace HotlineHyrule.Items
{
    [CreateAssetMenu(menuName = "Item/New Quest Item")]
    public class QuestItemData : ItemData
    {
        protected override bool IsItemNameReadOnly => true;
        public override string ItemName => name.Replace("item_", "");
    }
}
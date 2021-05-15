using System;

namespace HotlineHyrule.Items
{
    public class ItemEventArgs : EventArgs
    {
        public ItemData ItemData { get; }

        public ItemEventArgs(ItemData itemData) => ItemData = itemData;
    }
}
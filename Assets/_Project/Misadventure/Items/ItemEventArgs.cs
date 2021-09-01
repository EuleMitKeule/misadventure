using System;

namespace Misadventure.Items
{
    public class ItemEventArgs : EventArgs
    {
        public ItemData ItemData { get; }
        public ItemComponent ItemComponent { get; }

        public ItemEventArgs(ItemData itemData, ItemComponent itemComponent=null) => (ItemData, ItemComponent) = (itemData, itemComponent);
    }
}
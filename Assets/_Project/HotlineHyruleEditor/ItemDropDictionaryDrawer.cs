using HotlineHyrule.Extensions;
using HotlineHyrule.Items;
using UnityEditor;

namespace HotlineHyruleEditor
{
    [CustomPropertyDrawer(typeof(ItemDropDictionary))]
    public class ItemDropDictionaryDrawer : DictionaryDrawer<ItemData, float>
    {
        
    }
}
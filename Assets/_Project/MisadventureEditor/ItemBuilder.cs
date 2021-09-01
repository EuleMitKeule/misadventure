using System;
using System.Text.RegularExpressions;
using Misadventure.Items;
using Misadventure.Weapons;
using MisadventureEditor.Extensions;
using UnityEditor;
using UnityEngine;

namespace MisadventureEditor
{
    public static class ItemBuilder
    {
        public static string Path => "Assets/_Project/Prefabs/Items";

        public static ItemData CreateItem(string itemName, Type itemType, bool createPrefab)
        {
            if (itemName == null || !Regex.IsMatch(itemName, @"^([a-z\d\+\-])+(_([a-z\d\+\-])+)*$"))
            { 
                Debug.LogError("Item name is invalid. Use only lower-case letters and underscores.");
                return null;
            }

            if (!IsValidItemType(itemType)) return null;

            var fullItemName = $"item_{itemName}";
            var itemTypeName = itemType.Name.Replace("ItemData", "");
            var itemPath = $"{Path}/{itemTypeName}";
            var itemDataPath = $"{Path}/{itemTypeName}/{fullItemName}.asset";
            var itemPrefabPath = $"{Path}/{itemTypeName}/{fullItemName}.prefab";

            var itemData = (ItemData)ScriptableObject.CreateInstance(itemType);
            AssetDatabase.CreateAsset(itemData, itemDataPath);

            if (createPrefab)
            {
                var itemPrefab = new GameObject(fullItemName);
                itemPrefab.layer = LayerMask.NameToLayer("item");

                var spriteRenderer = itemPrefab.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "item";

                var circleCollider = itemPrefab.AddComponent<CircleCollider2D>();
                circleCollider.isTrigger = true;

                var itemComponent = itemPrefab.AddComponent<ItemComponent>();
                itemComponent.itemDatas.Add(itemData);

                PrefabUtility.SaveAsPrefabAsset(itemPrefab, itemPrefabPath);

                itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(itemPrefabPath);

                itemData.ItemPrefab = itemPrefab;
            }

            AssetDatabase.SaveAssets();

            return itemData;
        }

        public static void RenameItem(ItemData itemData, string newName)
        {
            if (newName == null || !Regex.IsMatch(newName, @"^([a-z\d\+\-])+(_([a-z\d\+\-])+)*$"))
            { 
                Debug.LogError("Item name is invalid. Use only lower-case letters and underscores.");
                return;
            }

            var itemPath = AssetDatabase.GetAssetPath(itemData);
            var itemPrefabPath = itemData.GetPrefabPath();
            AssetDatabase.RenameAsset(itemPrefabPath, $"item_{newName}");
            AssetDatabase.RenameAsset(itemPath, $"item_{newName}");
            AssetDatabase.SaveAssets();
        }

        public static void DeleteItem(ItemData itemData)
        {
            var itemPath = AssetDatabase.GetAssetPath(itemData);
            var itemPrefabPath = itemData.GetPrefabPath();
            AssetDatabase.DeleteAsset(itemPath);
            AssetDatabase.DeleteAsset(itemPrefabPath);
            AssetDatabase.SaveAssets();
        }

        static bool IsValidItemType(Type itemType) =>
            !itemType.IsAbstract &&
            itemType.IsSubclassOf(typeof(ItemData)) &&
            !itemType.IsSubclassOf(typeof(WeaponData));
    }
}
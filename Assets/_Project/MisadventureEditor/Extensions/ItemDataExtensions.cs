using Misadventure.Items;
using UnityEditor;
using UnityEngine;

namespace MisadventureEditor.Extensions
{
    public static class ItemDataExtensions
    {
        public static GameObject GetPrefab(this ItemData itemData) =>
            AssetDatabase.LoadAssetAtPath<GameObject>(itemData.GetPrefabPath());

        public static string GetPrefabPath(this ItemData itemData) =>
            AssetDatabase.GetAssetPath(itemData)?.Replace(".asset", ".prefab");
    }
}
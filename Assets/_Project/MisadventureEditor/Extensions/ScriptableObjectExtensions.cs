using UnityEditor;
using UnityEngine;

namespace MisadventureEditor.Extensions
{
    public static class ScriptableObjectExtensions
    {
        public static string GetPath(this ScriptableObject scriptableObject) =>
            AssetDatabase.GetAssetPath(scriptableObject);
    }
}
using UnityEditor;
using UnityEngine;

namespace HotlineHyruleEditor.Extensions
{
    public static class ScriptableObjectExtensions
    {
        public static string GetPath(this ScriptableObject scriptableObject) =>
            AssetDatabase.GetAssetPath(scriptableObject);
    }
}
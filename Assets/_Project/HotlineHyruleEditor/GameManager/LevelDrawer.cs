using System;
using HotlineHyrule.Level;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyruleEditor.GameManager
{
    public class LevelDrawer : ScriptableObjectDrawer<LevelData>
    {
        [BoxGroup("Tools/Main/Level")]
        [PropertySpace(5, 5)]
        [PropertyOrder(2)]
        [Button]
        void OpenScene()
        {
            if (!Selected) return;

            var sceneName = Selected.name
                .Replace("level", "scene");

            var scene = new Scene();

            try
            {
                scene = EditorSceneManager.OpenScene($"{Path}/{sceneName}.unity");
            }
            catch { }

            if (!scene.IsValid())
            {
                Debug.LogWarning($"Could not find scene \"{Path}/{sceneName}.unity\"!");
            }
        }

        public override string Path => $"Assets/_Project/Scenes/{GetCurrentSubfolder()}";

        public override void CreateNew()
        {
            var levelData = LevelBuilder.CreateLevel(NameForNew, Path);

            SetSelected(levelData);
        }

        public override void CreateNew(string overrideName, string overridePath)
        {
            if (overrideName == "") return;
            if (overridePath == "") return;

            var levelData = LevelBuilder.CreateLevel(overrideName, overridePath);

            SetSelected(levelData);
        }

        public override void DeleteSelected()
        {
            if (!Selected) return;

            var message = $"Are you sure you want to delete the level \"{Selected.name}\"?\nThis will also delete the scene file.";
            var isSure = EditorUtility.DisplayDialog("Delete Level", message, "Yes", "Cancel");

            if (!isSure) return;

            var levelName = Selected.name.Replace("level_", "");
            var scenePath = $"{GetCurrentDirectory()}\\scene_{levelName}.unity";

            AssetDatabase.DeleteAsset(scenePath);

            var assetPath = AssetDatabase.GetAssetPath(Selected);
            AssetDatabase.DeleteAsset(assetPath);

            AssetDatabase.SaveAssets();
        }

        public override void SetSelected(object item)
        {
            if (item is LevelData levelData)
            {
                Selected = levelData;
            }
        }

        public override void SetPath(string newPath) { }

        string GetCurrentSubfolder()
        {
            if (!Selected) return "";

            var selectedDirectory = GetCurrentDirectory();

            if (System.IO.Path.GetFullPath(selectedDirectory) ==
                System.IO.Path.GetFullPath(LevelBuilder.ParentPath)) return "";

            var subfolder = System.IO.Path.GetFileName(selectedDirectory);

            return subfolder;
        }

        string GetCurrentDirectory()
        {
            if (!Selected) return "";

            var selectedPath = AssetDatabase.GetAssetPath(Selected);
            var selectedDirectory = System.IO.Path.GetDirectoryName(selectedPath);

            return selectedDirectory;
        }
    }
}
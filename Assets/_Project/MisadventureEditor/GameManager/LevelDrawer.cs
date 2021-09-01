using Misadventure.Level;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MisadventureEditor.GameManager
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
                scene = EditorSceneManager.OpenScene($"{CurrentPath}/{sceneName}.unity");
            }
            catch { }

            if (!scene.IsValid())
            {
                Debug.LogWarning($"Could not find scene \"{CurrentPath}/{sceneName}.unity\"!");
            }
        }

        public override string Path => LevelBuilder.ParentPath;
        public string CurrentPath => $"{Path}/{GetCurrentSubfolder()}";

        public override void CreateNew()
        {
            var levelData = LevelBuilder.CreateLevel(NameForNew, CurrentPath);
            SetSelected(levelData);
        }

        public override void CreateNew(string overrideName, string overridePath)
        {
            var levelData = LevelBuilder.CreateLevel(overrideName, overridePath);
            SetSelected(levelData);
        }

        public override void RenameSelected()
        {
            var level = LevelBuilder.Rename(Selected, RenameName);
            SetSelected(level);
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
                NameForNew = levelData.name.Replace("level_", "");
                RenameName = levelData.name.Replace("level_", "");
            }
        }

        public override void SetPath(string newPath) { }
    }
}
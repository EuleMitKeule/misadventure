using System.IO;
using System.Text.RegularExpressions;
using Cinemachine;
using HotlineHyrule.Level;
using HotlineHyrule.Quests;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HotlineHyruleEditor
{
    public static class LevelBuilder
    {
        public static string ParentPath => $"Assets/_Project/Scenes";

        public static LevelData CreateLevel(string name, string path)
        {
            if (name == null || !Regex.IsMatch(name, @"^([a-z0-9])+(_([a-z0-9])+)*$"))
            { 
                Debug.LogError("Level name is invalid. Use only lower-case letters, numbers and underscores.");
                return null;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            var cameraPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Level/camera_main.prefab");
            var cameraObject = (GameObject)PrefabUtility.InstantiatePrefab(cameraPrefab, scene);

            PrefabUtility.UnpackPrefabInstance(cameraObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            var globalLightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Level/light_global.prefab");
            var globalLightObject = (GameObject)PrefabUtility.InstantiatePrefab(globalLightPrefab, scene);

            var gridPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Level/grid_base.prefab");
            var gridObject = (GameObject)PrefabUtility.InstantiatePrefab(gridPrefab, scene);
            gridObject.name = $"grid_{name}";

            PrefabUtility.UnpackPrefabInstance(gridObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            var questData = CreateQuest(name, path);

            var levelData = ScriptableObject.CreateInstance<LevelData>();
            levelData.questData = questData;
            AssetDatabase.CreateAsset(levelData, $"{path}/level_{name}.asset");

            var levelComponent = gridObject.GetComponent<LevelComponent>();
            levelComponent.levelData = levelData;

            var playerPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Entities/player.prefab");
            var playerObject = (GameObject) PrefabUtility.InstantiatePrefab(playerPrefab, scene);

            var mapBoundsObject = gridObject.transform.Find("collider_map_bounds");
            var mapBoundsCollider = mapBoundsObject.GetComponent<Collider2D>();
            var cinemachineConfiner2D = cameraObject.GetComponentInChildren<CinemachineConfiner2D>();
            cinemachineConfiner2D.m_BoundingShape2D = mapBoundsCollider;

            var cinemachineVirtualCamera = cameraObject.GetComponentInChildren<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = playerObject.transform;

            _ = new GameObject("enemies");
            _ = new GameObject("items");

            EditorSceneManager.SaveScene(scene, $"{path}/scene_{name}.unity");
            AssetDatabase.SaveAssets();

            return levelData;
        }
        
        public static LevelData Rename(LevelData level, string newName)
        {
            if (newName == null || !Regex.IsMatch(newName, @"^([a-z0-9])+(_([a-z0-9])+)*$"))
            { 
                Debug.LogError("Level name is invalid. Use only lower-case letters, numbers and underscores.");
                return null;
            }

            var path = AssetDatabase.GetAssetPath(level);
            AssetDatabase.RenameAsset(path, $"level_{newName}");

            var scenePath = path.Replace("level_", "scene_").Replace(".asset", ".unity");
            AssetDatabase.RenameAsset(scenePath, $"scene_{newName}");

            var questPath = path.Replace("level_", "quest_");
            AssetDatabase.RenameAsset(questPath, $"quest_{newName}");
            
            AssetDatabase.SaveAssets();
            return level;
        }

        public static QuestData CreateQuest(string name, string path)
        {
            var questData = ScriptableObject.CreateInstance<QuestData>();
            AssetDatabase.CreateAsset(questData, $"{path}/quest_{name}.asset");

            return questData;
        }
    }
}
using System.Text.RegularExpressions;
using Cinemachine;
using Misadventure.Level;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MisadventureEditor
{
    public class LevelCreatorWindow : EditorWindow
    {
        static string Name { get; set; }

        [MenuItem("Window/Level Creator")]
        public static void ShowWindow()
        {
            GetWindow<LevelCreatorWindow>("Level Creator");
        }

        void OnGUI()
        {
            Name = EditorGUILayout.TextField("Level Name:", Name);

            if (GUILayout.Button("Create Level"))
            {
                if (Name == null || !Regex.IsMatch(Name, @"^([a-z])+(_([a-z])+)*$"))
                {
                    Debug.LogError("Level name is invalid. Use only lower-case letters and underscores.");
                    return;
                }

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

                var cameraPrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Level/camera_main.prefab");
                var cameraObject = (GameObject)PrefabUtility.InstantiatePrefab(cameraPrefab, scene);

                var globalLightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Level/light_global.prefab");
                var globalLightObject = (GameObject)PrefabUtility.InstantiatePrefab(globalLightPrefab, scene);

                var gridPrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Level/grid_base.prefab");
                var gridObject = (GameObject)PrefabUtility.InstantiatePrefab(gridPrefab, scene);
                gridObject.name = $"grid_{Name}";

                PrefabUtility.UnpackPrefabInstance(gridObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                var levelData = CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(levelData, $"Assets/_Project/Scenes/level_{Name}.asset");

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

                EditorSceneManager.SaveScene(scene, $"Assets/_Project/Scenes/scene_{Name}.unity");
                AssetDatabase.SaveAssets();
            }
        }
    }
}
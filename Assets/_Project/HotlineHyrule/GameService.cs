using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyrule
{
    public static class GameService
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            SetupLevel();
        }

        static void SetupLevel()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            // if (Locator.GameComponent) return;
            if (!Locator.LevelComponent && SceneManager.GetActiveScene().name != "scene_menu") return;
#if UNITY_EDITOR
            var gamePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/game.prefab");
            PrefabUtility.InstantiatePrefab(gamePrefab, SceneManager.GetActiveScene());
#endif

            Locator.GameComponent.ReloadScene();
        }
    }
}
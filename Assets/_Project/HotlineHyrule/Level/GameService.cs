using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyrule.Level
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
            if (Locator.GameComponent) return;

            var gamePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/game.prefab");
            PrefabUtility.InstantiatePrefab(gamePrefab, SceneManager.GetActiveScene());

            Locator.GameComponent.ReloadScene();
        }
    }
}
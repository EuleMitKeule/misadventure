using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misadventure
{
    public class GameService : MonoBehaviour
    {
        [SerializeField]
        GameObject gamePrefab;

        void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            SetupLevel();
        }

        void SetupLevel()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (Locator.GameComponent) return;
            // if (!Locator.LevelComponent && SceneManager.GetActiveScene().name != "scene_menu") return;

            // var gamePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/game.prefab");
            // PrefabUtility.InstantiatePrefab(gamePrefab, SceneManager.GetActiveScene());

            Instantiate(gamePrefab);

            Locator.GameComponent.ReloadScene();
        }
    }
}
using System;
using System.Collections.Generic;
using HotlineHyrule.Attributes;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyrule
{
    public class GameComponent : MonoBehaviour
    {
        [Scene] [SerializeField] public List<string> scenes;

        int CurrentSceneIndex => scenes.IndexOf(SceneManager.GetActiveScene().name);

        bool IsLevel => scenes.Contains(SceneManager.GetActiveScene().name);

        public static event EventHandler<LevelEventArgs> LevelLoaded;
        public static event EventHandler<LevelEventArgs> LevelUnloaded;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Locator.GameComponent = this;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (IsLevel) SetupGame();
        }

        [ContextMenu("Load Next Scene")]
        public void LoadNextScene()
        {
            var currentLevelComponent = Locator.LevelComponent;
            if (currentLevelComponent) LevelUnloaded?.Invoke(this, new LevelEventArgs(currentLevelComponent.levelData));

            var nextSceneIndex = CurrentSceneIndex == -1 ? 0 : (CurrentSceneIndex + 1) % scenes.Count;
            SceneManager.LoadScene(scenes[nextSceneIndex]);
        }

        public void ReloadScene()
        {
            var currentLevelComponent = Locator.LevelComponent;
            if (currentLevelComponent) LevelUnloaded?.Invoke(this, new LevelEventArgs(currentLevelComponent.levelData));

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void SetupGame()
        {
            if (!Locator.LevelComponent) return;

            if (!Locator.LevelComponent.levelData)
            {
                Debug.LogWarning($"The loaded level {Locator.LevelComponent.name} has no level data assigned.");
                return;
            }

            var levelData = Locator.LevelComponent.levelData;
            LevelLoaded?.Invoke(this, new LevelEventArgs(levelData));
        }
    }
}
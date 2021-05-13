using System;
using System.Collections.Generic;
using HotlineHyrule.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyrule.Level
{
    public class GameComponent : MonoBehaviour
    {
        [Scene] [SerializeField] public List<string> scenes;

        int CurrentSceneIndex => scenes.IndexOf(SceneManager.GetActiveScene().name);

        public event EventHandler<LevelEventArgs> LevelLoaded;
        public event EventHandler<LevelEventArgs> LevelUnloaded;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Locator.GameComponent = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Start()
        {
            if (scenes.Contains(SceneManager.GetActiveScene().name))
            {
                SetupGame();
            }
        }

        [ContextMenu("Load Next Scene")]
        public void LoadNextScene()
        {
            var currentLevelComponent = Locator.LevelComponent;
            if (currentLevelComponent) LevelUnloaded?.Invoke(this, new LevelEventArgs(currentLevelComponent.levelData));

            var nextSceneIndex = CurrentSceneIndex == -1 ? 0 : (CurrentSceneIndex + 1) % scenes.Count;
            SceneManager.LoadScene(scenes[nextSceneIndex]);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            SetupGame();
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
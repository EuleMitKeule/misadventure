using System;
using System.Collections;
using System.Collections.Generic;
using HotlineHyrule.Attributes;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotlineHyrule
{
    public class GameComponent : MonoBehaviour
    {
        [Scene] [SerializeField] public List<string> scenes;

        [SerializeField] PlayerStateData playerStateData;

        int CurrentSceneIndex => scenes.IndexOf(SceneManager.GetActiveScene().name);
        bool IsLevel => scenes.Contains(SceneManager.GetActiveScene().name);

        public static event EventHandler<LevelEventArgs> LevelLoaded;
        public static event EventHandler<LevelEventArgs> LevelUnloaded;

        void Awake()
        {
            if (Locator.GameComponent) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            Locator.GameComponent = this;

            LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            if (e.PlayerStateData) playerStateData = e.PlayerStateData;
        }

        [ContextMenu("Load Next Scene")]
        public void LoadNextScene()
        {
            var currentLevelComponent = Locator.LevelComponent;

            if (currentLevelComponent)
            {
                var stateData =
                    Locator.PlayerComponent ? Locator.PlayerComponent.GetStateData() : null;
                var levelEventArgs = new LevelEventArgs(currentLevelComponent.levelData, stateData, false);
                LevelUnloaded?.Invoke(this, levelEventArgs);
            }
            else
            {
                LevelUnloaded?.Invoke(this, new LevelEventArgs(null, null, true));
            }

            var nextSceneIndex = CurrentSceneIndex == -1 ? 0 : (CurrentSceneIndex + 1) % scenes.Count;
            StartCoroutine(LoadSceneRoutine(scenes[nextSceneIndex]));
        }

        public void LoadMenuScene()
        {
            playerStateData = null;
            StartCoroutine(LoadSceneRoutine("scene_menu"));
        }

        public void ReloadScene()
        {
            var currentLevelComponent = Locator.LevelComponent;

            if (currentLevelComponent)
            {
                var stateData =
                    Locator.PlayerComponent ? Locator.PlayerComponent.GetStateData() : null;
                var levelEventArgs = new LevelEventArgs(currentLevelComponent.levelData, stateData, false);
                LevelUnloaded?.Invoke(this, levelEventArgs);
            }
            else
            {
                LevelUnloaded?.Invoke(this, new LevelEventArgs(null, null, true));
            }

            StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().name));
        }

        public void LoadFirstScene()
        {
            playerStateData = null;

            var currentLevelComponent = Locator.LevelComponent;

            if (currentLevelComponent)
            {
                var levelEventArgs = new LevelEventArgs(currentLevelComponent.levelData, null, false);
                LevelUnloaded?.Invoke(this, levelEventArgs);
            }

            StartCoroutine(LoadSceneRoutine(scenes[0]));
        }

        IEnumerator LoadSceneRoutine(string sceneName)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            if (IsLevel)
            {
                if (!Locator.LevelComponent.levelData)
                {
                    Logging.LogWarning($"The loaded level {Locator.LevelComponent.name} has no level data assigned.");
                    yield break;
                }
            }

            var levelData = Locator.LevelComponent ? Locator.LevelComponent.levelData : null;
            var isMenu = sceneName == "scene_menu";

            LevelLoaded?.Invoke(this, new LevelEventArgs(levelData, playerStateData, isMenu));
        }
    }
}
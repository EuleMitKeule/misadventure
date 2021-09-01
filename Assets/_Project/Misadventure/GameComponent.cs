using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HotlineHyrule.Attributes;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace HotlineHyrule
{
    public class GameComponent : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        public List<string> levels;
        [Scene]
        [SerializeField]
        public List<string> bossLevels;
        [SerializeField]
        int levelsToPlay;

        Queue<string> LevelPool { get; set; }
        string BossLevel { get; set; }

        float StartTime { get; set; }
        public float ElapsedTime => Time.time - StartTime;

        [SerializeField] PlayerStateData playerStateData;

        int CurrentSceneIndex => levels.IndexOf(SceneManager.GetActiveScene().name);
        bool IsLevel => levels.Contains(SceneManager.GetActiveScene().name);

        public static event EventHandler<LevelEventArgs> LevelLoaded;
        public static event EventHandler<LevelEventArgs> LevelUnloaded;

        public bool gameplayEnabled = false;

        void Awake()
        {
            if (Locator.GameComponent) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            Locator.GameComponent = this;

            ResetGame();

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

            var nextLevelName = LevelPool.Count > 0 ? LevelPool.Dequeue() : BossLevel;
            StartCoroutine(LoadSceneRoutine(nextLevelName));
        }

        void ResetGame()
        {
            LevelPool = new Queue<string>(levels.OrderBy(e => Guid.NewGuid()).Take(levelsToPlay));
            BossLevel = bossLevels.OrderBy(e => Guid.NewGuid()).First();
            playerStateData = null;
            StartTime = Time.time;
        }

        public void LoadMenuScene()
        {
            ResetGame();
            
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
            ResetGame();
            
            var currentLevelComponent = Locator.LevelComponent;

            if (currentLevelComponent)
            {
                var levelEventArgs = new LevelEventArgs(currentLevelComponent.levelData, null, false);
                LevelUnloaded?.Invoke(this, levelEventArgs);
            }

            StartCoroutine(LoadSceneRoutine(LevelPool.Dequeue()));
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
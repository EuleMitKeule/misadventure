using System;
using System.Collections.Generic;
using HotlineHyrule.Extensions;
using HotlineHyrule.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.Level
{
    /// <summary>
    /// Contains level information for the level (grid) it's attached to.
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class LevelComponent : MonoBehaviour
    {
        [SerializeField]
        public LevelData levelData;
        /// <summary>
        /// The prefab of the rain effect.
        /// </summary>
        [SerializeField]
        GameObject rainEffectPrefab;
        /// <summary>
        /// The prefab of the snow effect.
        /// </summary>
        [SerializeField]
        GameObject snowEffectPrefab;

        [SerializeField]
        List<GameObject> objectsToDestroyOnFinish = new List<GameObject>();

        DefaultControls DefaultControls { get; set; }

        public event EventHandler<EventArgs> LevelFinished;
        public event EventHandler<EventArgs> GameFinished;

        public Grid Grid { get; private set; }

        void Awake()
        {
            DefaultControls = new DefaultControls();
            Locator.LevelComponent = this;
            Grid = GetComponent<Grid>();
            
            var mainCamera = Camera.main;

            if (mainCamera)
            {
                if (levelData.IsRaining) Instantiate(rainEffectPrefab, mainCamera.transform);
                if (levelData.IsSnowing) Instantiate(snowEffectPrefab, mainCamera.transform);
            }

            DefaultControls.map_default.action_finish.performed += OnButtonFinish;
            
            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            if (!Locator.PlayerComponent) return;

            Locator.PlayerComponent.transform.position = e.LevelData.playerSpawnPosition.ToWorld();
            Locator.SoundComponent.PlayBGM(e.LevelData.bgmData);
        }

        public void FinishLevel(bool finishGame = false)
        {
            LevelFinished?.Invoke(this, EventArgs.Empty);
            DefaultControls.map_default.action_finish.Enable();

            foreach (var obj in objectsToDestroyOnFinish)
            {
                Destroy(obj);
            }

            if (!finishGame) return;
            GameFinished?.Invoke(this, EventArgs.Empty);
        }

        void OnButtonFinish(InputAction.CallbackContext context)
        {
            DefaultControls.map_default.action_finish.Disable();
            Locator.GameComponent.LoadNextScene();
        }
    }
}
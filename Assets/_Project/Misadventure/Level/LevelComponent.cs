using System;
using System.Collections.Generic;
using Misadventure.Extensions;
using Misadventure.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Misadventure.Level
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

        public event EventHandler<LevelFinishedEventArgs> LevelFinished;

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
            if (finishGame) DefaultControls.map_default.action_finish.performed += OnButtonMenu;
            else DefaultControls.map_default.action_finish.performed += OnButtonFinish;
            
            LevelFinished?.Invoke(this, new LevelFinishedEventArgs(finishGame));
            DefaultControls.map_default.action_finish.Enable();

            foreach (var obj in objectsToDestroyOnFinish)
            {
                Destroy(obj);
            }
        }

        void OnButtonFinish(InputAction.CallbackContext context)
        {
            DefaultControls.map_default.action_finish.performed -= OnButtonFinish;
            DefaultControls.map_default.action_finish.Disable();
            
            Locator.GameComponent.LoadNextScene();
        }

        void OnButtonMenu(InputAction.CallbackContext context)
        {
            DefaultControls.map_default.action_finish.performed -= OnButtonMenu;
            DefaultControls.map_default.action_finish.Disable();
            
            Locator.GameComponent.LoadMenuScene();
        }
    }
}
using Misadventure.Entities;
using Misadventure.Level;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Misadventure.UserInterface
{
    public class DeathComponent : MonoBehaviour
    {
        [SerializeField] InputAction restartAction;
        [SerializeField] InputAction exitAction;

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;

            restartAction.performed += OnButtonRestart;
            exitAction.performed += OnButtonExit;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu)
            {
                restartAction.Disable();
                exitAction.Disable();
                return;
            }

            if (!Locator.PlayerComponent) return;

            var healthComponent = Locator.PlayerComponent.GetComponent<HealthComponent>();
            healthComponent.HealthChanged += OnHealthChanged;
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (!e.IsKilled) return;

            restartAction.Enable();
            exitAction.Enable();
        }

        void OnButtonRestart(InputAction.CallbackContext context)
        {
            restartAction.Disable();
            exitAction.Disable();
            Locator.GameComponent.LoadFirstScene();
        }

        void OnButtonExit(InputAction.CallbackContext context)
        {
            restartAction.Disable();
            exitAction.Disable();
            Locator.GameComponent.LoadMenuScene();
        }
    }
}
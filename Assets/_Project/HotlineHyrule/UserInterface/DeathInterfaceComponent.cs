using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HotlineHyrule.UserInterface
{
    public class DeathInterfaceComponent : MonoBehaviour
    {
        [SerializeField] InputAction restartAction;
        [SerializeField] InputAction exitAction;

        Animator Animator { get; set; }
        CanvasGroup CanvasGroup { get; set; }

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;

            Animator = GetComponent<Animator>();
            CanvasGroup = GetComponent<CanvasGroup>();

            restartAction.performed += OnButtonRestart;
            exitAction.performed += OnButtonExit;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            Animator.SetBool("show", false);
            CanvasGroup.alpha = 0f;

            if (e.IsMenu) return;

            var healthComponent = Locator.PlayerComponent.GetComponent<HealthComponent>();
            healthComponent.HealthChanged += OnHealthChanged;
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {

        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (!e.IsKilled) return;

            Animator.SetBool("show", true);

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
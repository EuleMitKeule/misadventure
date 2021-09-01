using System;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class GameInterfaceComponent : MonoBehaviour
    {
        [SerializeField] public GameObject levelInfoParent;
        [SerializeField] public GameObject levelFinishedParent;
        [SerializeField] public GameObject gameFinishedParent;
        [SerializeField] public GameObject deathParent;

        Animator Animator { get; set; }
        Animator LevelInfoAnimator { get; set; }
        Animator LevelFinishedAnimator { get; set; }
        Animator GameFinishedAnimator { get; set; }
        Animator DeathAnimator { get; set; }

        void Awake()
        {
            Animator = GetComponent<Animator>();

            if (!levelInfoParent) levelInfoParent = transform.Find("parent_level_info").gameObject;
            if (levelInfoParent) LevelInfoAnimator = levelInfoParent.GetComponent<Animator>();
            if (!levelFinishedParent) levelFinishedParent = transform.Find("parent_level_finished").gameObject;
            if (levelFinishedParent) LevelFinishedAnimator = levelFinishedParent.GetComponent<Animator>();
            if (!gameFinishedParent) gameFinishedParent = transform.Find("parent_game_finished").gameObject;
            if (gameFinishedParent) GameFinishedAnimator = gameFinishedParent.GetComponent<Animator>();
            if (!deathParent) deathParent = transform.Find("parent_death").gameObject;
            if (deathParent) DeathAnimator = deathParent.GetComponent<Animator>();

            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu)
            {
                Animator.SetBool("showFinished", false);
                Animator.SetBool("showDeath", false);
                Animator.SetBool("showInfo", false);
                Animator.SetBool("showGameFinished", false);
                return;
            }

            if (!Locator.LevelComponent) return;
            Locator.LevelComponent.LevelFinished += OnLevelFinished;

            if (!Locator.PlayerComponent) return;
            Locator.PlayerComponent.MovementStarted += OnMovementStarted;

            var healthComponent = Locator.PlayerComponent.GetComponent<HealthComponent>();
            healthComponent.HealthChanged += OnHealthChanged;

            Animator.SetBool("showInfo", true);
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            Animator.SetBool("showFinished", false);
            Animator.SetBool("showDeath", false);
            Animator.SetBool("showInfo", false);
            Animator.SetBool("showGameFinished", false);

            if (e.IsMenu) return;

            if (!Locator.PlayerComponent) return;
            Locator.PlayerComponent.MovementStarted -= OnMovementStarted;
        }

        void OnLevelFinished(object sender, LevelFinishedEventArgs e)
        {
            Animator.SetBool("showFinished", !e.FinishGame);
            Animator.SetBool("showGameFinished", e.FinishGame);
        }

        void OnHealthChanged(object sender, HealthEventArgs e)
        {
            if (!e.IsKilled) return;

            Animator.SetBool("showDeath", true);
        }

        void OnMovementStarted(object sender, EventArgs e)
        {
            Animator.SetBool("showInfo", false);
            Locator.GameComponent.gameplayEnabled = true;
        }

        public void ShowLevelInfo()
        {
            if (LevelInfoAnimator) LevelInfoAnimator.SetTrigger("show");
        }
        
        public void HideLevelInfo()
        {
            if (LevelInfoAnimator) LevelInfoAnimator.SetTrigger("hide");
        }

        public void ShowLevelFinished()
        {
            if (LevelFinishedAnimator) LevelFinishedAnimator.SetTrigger("show");
        }

        public void ShowGameFinished()
        {
            if (GameFinishedAnimator) GameFinishedAnimator.SetTrigger("show");
        }

        public void HideLevelFinished()
        {
            if (LevelFinishedAnimator) LevelFinishedAnimator.SetTrigger("hide");
        }

        public void HideGameFinished()
        {
            if (GameFinishedAnimator) GameFinishedAnimator.SetTrigger("hide");
        }

        public void ShowDeath()
        {
            if (DeathAnimator) DeathAnimator.SetBool("show", true);
        }

        public void HideDeath()
        {
            if (DeathAnimator) DeathAnimator.SetBool("show", false);
        }

        public void EnablePlayerMovement()
        {
            if (Locator.PlayerComponent) Locator.PlayerComponent.EnableMovement();
        }
    }
}
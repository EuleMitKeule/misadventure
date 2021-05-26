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

        Animator Animator { get; set; }
        Animator LevelInfoAnimator { get; set; }
        Animator LevelFinishedAnimator { get; set; }

        void Awake()
        {
            Animator = GetComponent<Animator>();

            if (!levelInfoParent) levelInfoParent = transform.Find("parent_level_info").gameObject;
            if (levelInfoParent) LevelInfoAnimator = levelInfoParent.GetComponent<Animator>();
            if (!levelFinishedParent) levelFinishedParent = transform.Find("parent_level_finished").gameObject;
            if (levelFinishedParent) LevelFinishedAnimator = levelFinishedParent.GetComponent<Animator>();

            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;

            Locator.LevelComponent.LevelFinished += OnLevelFinished;
            Locator.PlayerComponent.MovementStarted += OnMovementStarted;

            Animator.SetBool("showInfo", true);
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            Animator.SetBool("showFinished", false);

            if (e.IsMenu) return;
            Locator.PlayerComponent.MovementStarted -= OnMovementStarted;
        }

        void OnLevelFinished(object sender, EventArgs e)
        {
            Animator.SetBool("showFinished", true);
        }

        void OnMovementStarted(object sender, EventArgs e)
        {
            Animator.SetBool("showInfo", false);
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

        public void HideLevelFinished()
        {
            if (LevelFinishedAnimator) LevelFinishedAnimator.SetTrigger("hide");
        }

        public void EnablePlayerMovement()
        {
            if (Locator.PlayerComponent) Locator.PlayerComponent.SetState(Locator.PlayerComponent.IdleState);
        }
    }
}
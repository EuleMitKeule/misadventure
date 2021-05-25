using System;
using HotlineHyrule.Entities;
using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class GameInterfaceComponent : MonoBehaviour
    {
        [SerializeField] public GameObject levelInfoParent;

        Animator Animator { get; set; }
        Animator LevelInfoAnimator { get; set; }

        void Awake()
        {
            Animator = GetComponent<Animator>();

            if (!levelInfoParent) levelInfoParent = transform.Find("parent_level_info").gameObject;
            if (levelInfoParent) LevelInfoAnimator = levelInfoParent.GetComponent<Animator>();

            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;

            Locator.PlayerComponent.MovementStarted += OnMovementStarted;

            Animator.SetTrigger("showInfo");
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu) return;
            Locator.PlayerComponent.MovementStarted -= OnMovementStarted;
        }

        void OnMovementStarted(object sender, EventArgs e)
        {
            Animator.SetTrigger("hideInfo");
        }

        public void ShowLevelInfo()
        {
            if (LevelInfoAnimator) LevelInfoAnimator.SetTrigger("show");
        }
        
        public void HideLevelInfo()
        {
            if (LevelInfoAnimator) LevelInfoAnimator.SetTrigger("hide");
        }
    }
}
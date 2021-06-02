using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class MenuInterfaceComponent : MonoBehaviour
    {
        CanvasGroup CanvasGroup { get; set; }

        void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();

            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu)
            {
                CanvasGroup.alpha = 1f;
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;

                return;
            }

            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        public void OnButtonStart()
        {
            Locator.GameComponent.LoadNextScene();
        }
    }
}
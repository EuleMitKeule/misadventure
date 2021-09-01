using Misadventure.Level;
using UnityEngine;

namespace Misadventure.UserInterface
{
    public class UserInterfaceComponent : MonoBehaviour
    {
        [SerializeField] Canvas mainCanvas;
        [SerializeField] Canvas menuCanvas;

        CanvasGroup mainCanvasGroup;

        void Awake()
        {
            if (!mainCanvas)
            {
                var mainCanvasObject = transform.Find("canvas_game");
                if (mainCanvasObject) mainCanvas = mainCanvasObject.GetComponent<Canvas>();
                if (mainCanvasObject) mainCanvasGroup = mainCanvasObject.GetComponent<CanvasGroup>();
            }

            if (!menuCanvas)
            {
                var menuCanvasObject = transform.Find("canvas_menu");
                if (menuCanvasObject) menuCanvas = menuCanvasObject.GetComponent<Canvas>();
            }

            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (e.IsMenu)
            {
                mainCanvasGroup.alpha = 0f;
                mainCanvasGroup.interactable = false;
                mainCanvasGroup.blocksRaycasts = false;
            }
            else
            {
                mainCanvasGroup.alpha = 1f;
                mainCanvasGroup.interactable = true;
                mainCanvasGroup.blocksRaycasts = true;
            }

            mainCanvas.worldCamera = Camera.main;
            menuCanvas.worldCamera = Camera.main;
        }
    }
}
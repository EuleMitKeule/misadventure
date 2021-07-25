using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class UserInterfaceComponent : MonoBehaviour
    {
        [SerializeField] Canvas mainCanvas;
        [SerializeField] Canvas menuCanvas;

        void Awake()
        {
            if (!mainCanvas)
            {
                var mainCanvasObject = transform.Find("canvas_game");
                if (mainCanvasObject) mainCanvas = mainCanvasObject.GetComponent<Canvas>();
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
            mainCanvas.worldCamera = Camera.main;
            menuCanvas.worldCamera = Camera.main;
        }
    }
}
using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class UserInterfaceComponent : MonoBehaviour
    {
        [SerializeField] Canvas mainCanvas;

        void Awake()
        {
            if (!mainCanvas)
            {
                var mainCanvasObject = transform.Find("canvas_main");
                if (mainCanvasObject) mainCanvas = mainCanvasObject.GetComponent<Canvas>();
            }

            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            mainCanvas.worldCamera = Camera.main;
        }
    }
}
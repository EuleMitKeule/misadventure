using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class LevelInformationInterfaceComponent : MonoBehaviour
    {
        void Awake()
        {
            Locator.GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            Debug.Log(e.LevelData.areaName);
            Debug.Log(e.LevelData.areaText);
        }
    }
}
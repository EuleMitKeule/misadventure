using HotlineHyrule.Level;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class LevelInformationInterfaceComponent : MonoBehaviour
    {
        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (!e.LevelData) return;
            Debug.Log(e.LevelData.areaName);
            Debug.Log(e.LevelData.areaText);
        }
    }
}
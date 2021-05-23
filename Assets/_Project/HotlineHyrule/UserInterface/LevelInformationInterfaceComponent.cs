using HotlineHyrule.Level;
using HotlineHyrule.Quests;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class LevelInformationInterfaceComponent : MonoBehaviour
    {
        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnQuestTargetReached(object sender, QuestTargetEventArgs e)
        {
            Debug.Log(e.QuestTarget.targetText);
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (Locator.QuestComponent) Locator.QuestComponent.QuestTargetReached += OnQuestTargetReached;

            if (!e.LevelData) return;
            Debug.Log(e.LevelData.areaName);
            Debug.Log(e.LevelData.areaText);
            if (e.LevelData.questData) Debug.Log(e.LevelData.questData.questText);
        }
    }
}
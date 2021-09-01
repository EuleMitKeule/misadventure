using HotlineHyrule.Level;
using HotlineHyrule.Quests;
using TMPro;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class LevelInformationInterfaceComponent : MonoBehaviour
    {
        Animator Animator { get; set; }
        
        void Awake()
        {
            Animator = GetComponent<Animator>();
            
            GameComponent.LevelLoaded += OnLevelLoaded;
        }

        void OnQuestTargetReached(object sender, QuestTargetEventArgs e)
        {

        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (Locator.QuestComponent) Locator.QuestComponent.QuestTargetReached += OnQuestTargetReached;

            if (!e.LevelData) return;
            if (!e.LevelData.questData) return;

            var labels = GetComponentsInChildren<TextMeshProUGUI>();
            labels[0].text = e.LevelData.areaName;
            labels[1].text = e.LevelData.areaText;
            labels[2].text = e.LevelData.questData.questText;
            labels[3].text = e.LevelData.questData.questTargetText;
        }
    }
}
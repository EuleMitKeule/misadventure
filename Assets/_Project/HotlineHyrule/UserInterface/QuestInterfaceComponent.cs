using System.Collections.Generic;
using HotlineHyrule.Level;
using HotlineHyrule.Quests;
using TMPro;
using UnityEngine;

namespace HotlineHyrule.UserInterface
{
    public class QuestInterfaceComponent : MonoBehaviour
    {
        [SerializeField] GameObject questTargetPrefab;
        
        QuestData QuestData { get; set; }
        Dictionary<QuestTarget, GameObject> TargetToTargetObject { get; set; }

        void Awake()
        {
            GameComponent.LevelLoaded += OnLevelLoaded;
            GameComponent.LevelUnloaded += OnLevelUnloaded;
        }

        void OnLevelLoaded(object sender, LevelEventArgs e)
        {
            if (!e.LevelData) return;
            if (!e.LevelData.questData) return;
            if (!questTargetPrefab) return;

            QuestData = e.LevelData.questData;
            Locator.QuestComponent.QuestTargetReached += OnQuestTargetReached;

            TargetToTargetObject = new Dictionary<QuestTarget, GameObject>();

            foreach (var questTarget in e.LevelData.questData.questTargets)
            {
                var questTargetObject = Instantiate(questTargetPrefab, transform);
                var label = questTargetObject.GetComponentInChildren<TextMeshProUGUI>();
                var questTargetText =
                    $"{(questTarget.isRequired ? "" : "(")}{questTarget.shortTargetText}{(questTarget.isRequired ? "" : ")")}";
                label.text = $"{questTargetText} ~";
                
                TargetToTargetObject.Add(questTarget, questTargetObject);
            }
        }

        void OnLevelUnloaded(object sender, LevelEventArgs e)
        {
            Locator.QuestComponent.QuestTargetReached -= OnQuestTargetReached;
        }

        void OnQuestTargetReached(object sender, QuestTargetEventArgs e)
        {
            if (!QuestData.questTargets.Contains(e.QuestTarget)) return;

            var questTargetObject = TargetToTargetObject[e.QuestTarget];
            var label = questTargetObject.GetComponentInChildren<TextMeshProUGUI>();
            label.fontStyle = FontStyles.Strikethrough;
        }
    }
}